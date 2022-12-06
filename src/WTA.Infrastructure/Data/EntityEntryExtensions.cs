using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using WTA.Application.Abstractions.Domain;
using WTA.Application.Abstractions.Extensions;
using WTA.Application.Domain;

namespace WTA.Infrastructure.Data;

public static class EntityEntryExtensions
{
  public static void UpdateConcurrencyStamp(this IEnumerable<EntityEntry> list)
  {
    foreach (var item in list.Where(o => o.State == EntityState.Added || o.State == EntityState.Modified))
    {
      if (item.Entity is IConcurrencyStamp entity)
      {
        entity.ConcurrencyStamp = Guid.NewGuid().ToString();
      }
    }
  }

  public static void UpdateTenant(this IEnumerable<EntityEntry> list, string? tenant)
  {
    foreach (var item in list.Where(o => o.State == EntityState.Added))
    {
      if (item.Entity is ITenant entity)
      {
        entity.Tenant = tenant;
      }
    }
  }

  public static void UpdateAudit(this IEnumerable<EntityEntry> list, string? user)
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

  public static void UpdateEvent(this IEnumerable<EntityEntry> entries, AppDbContext dbContext)
  {
    foreach (var item in entries)
    {
      if (item.Entity.GetType() != typeof(Event))
      {
        if (item.State == EntityState.Added || item.State == EntityState.Modified || item.State == EntityState.Deleted)
        {
          dbContext.Set<Event>().Add(new Event
          {
            Date = DateTimeOffset.Now,
            Entity = item.Entity.GetType().Name,
            EventType = item.State.ToString(),
            Original = item.State == EntityState.Added ? null : item.OriginalValues.ToObject().ToJson(),
            Current = item.State == EntityState.Deleted ? null : item.Entity.ToJson()
          });
        }
      }
    }
  }
}
