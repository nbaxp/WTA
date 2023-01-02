namespace WTA.Application.Abstractions.Include;

public interface IIncludable<out TEntity, out TProperty> : IQueryable<TEntity>
{
    IQueryable<TEntity> Query { get; }
}
