using Microsoft.Extensions.DependencyInjection;

namespace WTA.Application.Abstractions.Data;

public static class IQueryableExtensions
{
    public static IQueryable<T> Where<T>(this IQueryable<T> source, string queryString, params object[] args)
    {
        using var scope = App.Current.Services!.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ILinqDynamic>();
        return service.Where(source, queryString, args);
    }

    public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, string ordering, params object[] args)
    {
        using var scope = App.Current.Services!.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ILinqDynamic>();
        return service.OrderBy(source, ordering, args);
    }

    public static IQueryable<TEntity> Query<TEntity, TModel>(this IQueryable<TEntity> source, TModel model)
    {
        using var scope = App.Current.Services!.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ILinqDynamic>();
        return service.Query(source, model);
    }
}
