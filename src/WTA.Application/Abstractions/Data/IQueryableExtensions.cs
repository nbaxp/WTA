using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;

namespace WTA.Application.Abstractions.Data;

public static class IQueryableExtensions
{
    public static IIncludable<TEntity, TProperty> Include<TEntity, TProperty>(
           this IQueryable<TEntity> source,
           Expression<Func<TEntity, TProperty>> path)
           where TEntity : class
    {
        using var scope = AppContext.Current.Services!.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ILinqInclude>();
        return service.Include(source, path);
    }

    public static IIncludable<TEntity, TProperty> ThenInclude<TEntity, TPreviousProperty, TProperty>(
            this IIncludable<TEntity, IEnumerable<TPreviousProperty>> source,
            Expression<Func<TPreviousProperty, TProperty>> path)
            where TEntity : class
    {
        using var scope = AppContext.Current.Services!.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ILinqInclude>();
        return service.ThenInclude(source, path);
    }

    public static IIncludable<TEntity, TProperty> ThenInclude<TEntity, TPreviousProperty, TProperty>(
        this IIncludable<TEntity, TPreviousProperty> source,
        Expression<Func<TPreviousProperty, TProperty>> path)
        where TEntity : class
    {
        using var scope = AppContext.Current.Services!.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ILinqInclude>();
        return service.ThenInclude(source, path);
    }

    public static IQueryable<T> Where<T>(this IQueryable<T> source, string queryString, params object[] args)
    {
        using var scope = AppContext.Current.Services!.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ILinqDynamic>();
        return service.Where(source, queryString, args);
    }

    public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, string ordering, params object[] args)
    {
        using var scope = AppContext.Current.Services!.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ILinqDynamic>();
        return service.OrderBy(source, ordering, args);
    }
}
