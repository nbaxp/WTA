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
