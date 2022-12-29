using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using WTA.Application.Abstractions.Data;

namespace WTA.Infrastructure.Data;

public class DefaultLinqInclude : ILinqInclude
{
    public IIncludable<TEntity, TProperty> Include<TEntity, TProperty>(IQueryable<TEntity> source, Expression<Func<TEntity, TProperty>> path) where TEntity : class
    {
        return new IncludableQueryable<TEntity, TProperty>(EntityFrameworkQueryableExtensions.Include(source, path));
    }

    public IIncludable<TEntity, TProperty> ThenInclude<TEntity, TPreviousProperty, TProperty>(
          IIncludable<TEntity, IEnumerable<TPreviousProperty>> source,
          Expression<Func<TPreviousProperty, TProperty>> path)
          where TEntity : class
    {
        var query = EntityFrameworkQueryableExtensions.ThenInclude((IIncludableQueryable<TEntity, IEnumerable<TPreviousProperty>>)source.Query, path);
        return new IncludableQueryable<TEntity, TProperty>(query);
    }

    public IIncludable<TEntity, TProperty> ThenInclude<TEntity, TPreviousProperty, TProperty>(
      IIncludable<TEntity, TPreviousProperty> source,
      Expression<Func<TPreviousProperty, TProperty>> path)
      where TEntity : class
    {
        var query = EntityFrameworkQueryableExtensions.ThenInclude((IIncludableQueryable<TEntity, TPreviousProperty>)source.Query, path);
        return new IncludableQueryable<TEntity, TProperty>(query);
    }
}
