namespace WTA.Application.Abstractions.Data;

public interface ILinqDynamic
{
    IQueryable<T> Where<T>(IQueryable<T> source, string queryString, object[] args);

    IQueryable<TEntity> Query<TEntity, TModel>(IQueryable<TEntity> source, TModel model);

    IQueryable<T> OrderBy<T>(IQueryable<T> source, string ordering, params object[] args);
}
