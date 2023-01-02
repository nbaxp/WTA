using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;

namespace WTA.Application.Abstractions.Include;

public static class IQueryableExtensions
{
    public static IIncludable<TEntity, TProperty> Include<TEntity, TProperty>(
           this IQueryable<TEntity> source,
           Expression<Func<TEntity, TProperty>> path)
           where TEntity : class
    {
        using var scope = ApplicationContext.Current.Services!.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ILinqInclude>();
        return service.Include(source, path);
    }

    public static IIncludable<TEntity, TProperty> ThenInclude<TEntity, TPreviousProperty, TProperty>(
            this IIncludable<TEntity, IEnumerable<TPreviousProperty>> source,
            Expression<Func<TPreviousProperty, TProperty>> path)
            where TEntity : class
    {
        using var scope = ApplicationContext.Current.Services!.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ILinqInclude>();
        return service.ThenInclude(source, path);
    }

    public static IIncludable<TEntity, TProperty> ThenInclude<TEntity, TPreviousProperty, TProperty>(
        this IIncludable<TEntity, TPreviousProperty> source,
        Expression<Func<TPreviousProperty, TProperty>> path)
        where TEntity : class
    {
        using var scope = ApplicationContext.Current.Services!.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ILinqInclude>();
        return service.ThenInclude(source, path);
    }
}
