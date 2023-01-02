using System.Linq.Expressions;
using System.Reflection;

namespace WTA.Application.Abstractions.Expressions;

public static class ObjectToExpressionExtensions
{
    public static Expression<Func<TEntity, bool>>? ToExpression<TEntity, TModel>(this TModel model)
    {
        Expression<Func<TEntity, bool>> expression = o => true;
        var properties = model.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);
        foreach (var property in properties)
        {
            var propertyName = property.Name;
            var propertyValue = property.GetValue(model, null);
            if (propertyValue != null)
            {
                var propertyType = property.PropertyType;
            }
        }
        return null;
    }

    private static Expression<Func<TEntity, bool>> GetExpression<TEntity, TModel>(TModel model, string propertyName, string propertyValue)
    {
        var parameter = Expression.Parameter(typeof(TEntity), "o");
        var member = Expression.PropertyOrField(parameter, propertyName);
        var constant = Expression.Constant(propertyValue);
        return Expression.Lambda<Func<TEntity, bool>>(Expression.Equal(member, constant), parameter);
    }

    public static string ToPredicate<TEntity, TModel>(this TModel model)
    {
        var predicate = string.Empty;
        var properties = model.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);
        foreach (var property in properties)
        {
            var propertyName = property.Name;
            var propertyValue = property.GetValue(model, null);
            if (propertyValue != null)
            {
                predicate += $" and ";
            }
        }
        return predicate;
    }
}
