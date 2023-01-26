using LinqToDB.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WTA.Application.Abstractions;
using WTA.Application.Abstractions.Domain;
using WTA.Application.Abstractions.EventBus;
using WTA.Application.Abstractions.Extensions;
using WTA.Application.Domain.System;

namespace WTA.Infrastructure.Data;

public class DefaultDbContext : DbContext
{
    private readonly IServiceProvider _serviceProvider;

    static DefaultDbContext()
    {
        LinqToDBForEFTools.Initialize();
    }

    public static readonly ILoggerFactory DefaultLoggerFactory = LoggerFactory.Create(builder => { builder.AddConsole(); });

    public DefaultDbContext(DbContextOptions options, IServiceProvider serviceProvider) : base(options)
    {
        _serviceProvider = serviceProvider;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseLoggerFactory(DefaultLoggerFactory);
        optionsBuilder.EnableSensitiveDataLogging();
        optionsBuilder.EnableDetailedErrors();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Config();
        modelBuilder.ConfigComment()
            .ConfigKey()
            .ConfigConcurrencyStamp()
            .ConfigTreeNode();
        //.ConfigTenant(_tenant);
    }

    public override int SaveChanges()
    {
        var scope = this._serviceProvider.CreateScope();
        var services = scope.ServiceProvider;
        var entries = GetEntries();
        BeforeSave(entries, services);
        var result = base.SaveChanges();
        AfterSave(entries, services);
        return result;
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var scope = _serviceProvider.CreateScope();
        var services = scope.ServiceProvider;
        var entries = GetEntries();
        BeforeSave(entries, services);
        var result = await base.SaveChangesAsync(cancellationToken);
        AfterSave(entries, services);
        return result;
    }

    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        var scope = this._serviceProvider.CreateScope();
        var services = scope.ServiceProvider;
        var entries = GetEntries();
        BeforeSave(entries, services);
        var result = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        AfterSave(entries, services);
        return result;
    }

    private void BeforeSave(List<EntityEntry> entries, IServiceProvider services)
    {
        //
        UpdateConcurrencyStamp(entries);
        //
        var httpContext = services.GetRequiredService<IHttpContextAccessor>().HttpContext;
        var tenantService = services.GetRequiredService<ITenantService>();
        UpdateAudit(entries, httpContext?.User?.Identity?.Name);
        //
        UpdateTenant(entries, tenantService.Tenant);
        //
        foreach (var entry in entries)
        {
            if (entry.Entity.GetType() != typeof(EntityEvent))
            {
                if (entry.State == EntityState.Added || entry.State == EntityState.Modified || entry.State == EntityState.Deleted)
                {
                    this.Set<EntityEvent>().Add(new EntityEvent
                    {
                        Date = DateTimeOffset.Now,
                        Entity = entry.Entity.GetType().Name,
                        EventType = entry.State.ToString(),
                        Original = entry.State == EntityState.Added ? null : entry.OriginalValues.ToObject().ToJson(),
                        Current = entry.State == EntityState.Deleted ? null : entry.Entity.ToJson()
                    });
                }
            }
        }
    }

    private void AfterSave(List<EntityEntry> entries, IServiceProvider services)
    {
        foreach (var entry in entries)
        {
            if (entry.Entity.GetType() != typeof(EntityEvent))
            {
                var events = new List<object>();
                if (entry.State == EntityState.Added)
                {
                    events.Add(Activator.CreateInstance(typeof(EntityCreatedEvent<>).MakeGenericType(entry.Entity.GetType()), entry.Entity)!);
                }
                else if (entry.State == EntityState.Modified)
                {
                    events.Add(Activator.CreateInstance(typeof(EntityUpdatedEvent<>).MakeGenericType(entry.Entity.GetType()), entry.Entity, entry.OriginalValues.ToObject())!);
                }
                else if (entry.State == EntityState.Deleted)
                {
                    events.Add(Activator.CreateInstance(typeof(EntityDeletedEvent<>).MakeGenericType(entry.Entity.GetType()), entry.Entity)!);
                }
                var publisher = services.GetRequiredService<IEventPublisher>();
                events.ForEach(o => publisher.Publish(o));
            }
        }
    }

    private List<EntityEntry> GetEntries()
    {
        this.ChangeTracker.DetectChanges();
        var entries = this.ChangeTracker.Entries().ToList();
        return entries;
    }

    private void UpdateConcurrencyStamp(IEnumerable<EntityEntry> list)
    {
        foreach (var item in list.Where(o => o.State == EntityState.Added || o.State == EntityState.Modified))
        {
            if (item.Entity is IConcurrencyStamp entity)
            {
                entity.ConcurrencyStamp = Guid.NewGuid().ToString();
            }
        }
    }

    private void UpdateTenant(IEnumerable<EntityEntry> list, string? tenant)
    {
        foreach (var item in list.Where(o => o.State == EntityState.Added))
        {
            if (item.Entity is ITenant entity)
            {
                entity.Tenant = tenant;
            }
        }
    }

    private void UpdateAudit(IEnumerable<EntityEntry> list, string? user)
    {
        foreach (var item in list.Where(o => o.State == EntityState.Added || o.State == EntityState.Modified))
        {
            if (item.Entity is IAudit entity)
            {
                if (item.State == EntityState.Added)
                {
                    entity.CreatedAt = DateTimeOffset.Now;
                    entity.CreatedBy = user;
                }
                else if (item.State == EntityState.Modified)
                {
                    entity.ModifiedAt = DateTimeOffset.Now;
                    entity.ModifiedBy = user;
                }
            }
        }
    }
}
