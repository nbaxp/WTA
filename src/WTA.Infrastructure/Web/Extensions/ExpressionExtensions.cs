using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;

namespace WTA.Infrastructure.Web.Extensions;

public static class ExpressionExtensions
{
    public static Expression<Func<TEntity, bool>>? CreateExpression<TEntity>(this object model) where TEntity : class
    {
        if (model == null)
        {
            return null;
        }

        var entityType = typeof(TEntity);
        var modelType = model.GetType();
        var properties = modelType.GetProperties().Where(o => o.GetCustomAttribute<KeyAttribute>() != null).ToList();
        Expression<Func<TEntity, bool>>? predicate = null;

        foreach (var item in properties)
        {
            if (entityType.GetProperty(item.Name) != null)
            {
                var parameter = Expression.Parameter(typeof(TEntity), "p");
                var memberExpression = Expression.Property(parameter, item.Name);
                var value = Expression.Constant(item.GetValue(model));
                var binaryExpression = Expression.Equal(memberExpression, value);
                var filter = Expression.Lambda<Func<TEntity, bool>>(binaryExpression, parameter);
                predicate = predicate == null ? filter : predicate.And(filter);
            }
        }

        return predicate;
    }
}
