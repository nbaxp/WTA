namespace WTA.Application.Abstractions.Data;

public interface IIncludable<out TEntity, out TProperty> : IQueryable<TEntity>
{
  IQueryable<TEntity> Query { get; }
}
