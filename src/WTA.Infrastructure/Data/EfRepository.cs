using Microsoft.EntityFrameworkCore;
using WTA.Application.Abstractions.Data;
using WTA.Application.Abstractions.Domain;

namespace WTA.Infrastructure.Data;

public class EfRepository<T> : IRepository<T> where T : BaseEntity
{
  private readonly DbContext _efDbContext;

  public EfRepository(DbContext efDbContext)
  {
    this._efDbContext = efDbContext;
  }

  public ValueTask<T?> FindAsync(Guid id, CancellationToken cancellationToken = default)
  {
    return _efDbContext.Set<T>().FindAsync(id, cancellationToken);
  }

  public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
  {
    await _efDbContext.Set<T>().AddAsync(entity, cancellationToken);
  }

  public Task AddRangeAsync(T[] entities, CancellationToken cancellationToken = default)
  {
    return _efDbContext.AddRangeAsync(entities, cancellationToken);
  }

  public void Remove(T entity)
  {
    _efDbContext.Remove(entity);
  }

  public void RemoveAsync(T[] entities)
  {
    _efDbContext.RemoveRange(entities);
  }

  public IQueryable<T> Tracking()
  {
    return _efDbContext.Set<T>();
  }

  public IQueryable<T> NoTracking()
  {
    return _efDbContext.Set<T>().AsNoTracking();
  }

  public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
  {
    return _efDbContext.SaveChangesAsync(cancellationToken);
  }
}
