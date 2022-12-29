using LinqToDB.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WTA.Application.Abstractions;

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
        this._serviceProvider = serviceProvider;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseLoggerFactory(DefaultLoggerFactory);
        optionsBuilder.EnableSensitiveDataLogging();
        optionsBuilder.EnableDetailedErrors();
    }

    public override int SaveChanges()
    {
        this.SaveChangesInternal();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        this.SaveChangesInternal();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        this.SaveChangesInternal();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
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

    private void SaveChangesInternal()
    {
        this.ChangeTracker.DetectChanges();
        var entries = this.ChangeTracker.Entries().ToList();
        entries.UpdateConcurrencyStamp();
        using var scope = this._serviceProvider.CreateScope();
        var httpContext = scope.ServiceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext;
        var tenantService = scope.ServiceProvider.GetRequiredService<ITenantService>();
        entries.UpdateAudit(httpContext?.User?.Identity?.Name);
        entries.UpdateTenant(tenantService.Tenant);
        entries.UpdateEvent(this);
    }
}
