using System.Linq.Expressions;

namespace WTA.Application.Abstractions.Data;

public interface ILinqInclude
{
    IIncludable<TEntity, TProperty> Include<TEntity, TProperty>(
      IQueryable<TEntity> source,
      Expression<Func<TEntity, TProperty>> path)
      where TEntity : class;

    IIncludable<TEntity, TProperty> ThenInclude<TEntity, TPreviousProperty, TProperty>(
      IIncludable<TEntity, IEnumerable<TPreviousProperty>> source,
      Expression<Func<TPreviousProperty, TProperty>> navigationPropertyPath)
      where TEntity : class;

    IIncludable<TEntity, TProperty> ThenInclude<TEntity, TPreviousProperty, TProperty>(
      IIncludable<TEntity, TPreviousProperty> source,
      Expression<Func<TPreviousProperty, TProperty>> path)
      where TEntity : class;
}
