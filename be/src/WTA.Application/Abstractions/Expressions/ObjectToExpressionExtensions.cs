using System.Linq.Expressions;
using System.Reflection;

namespace WTA.Application.Abstractions.Expressions;

public static class ObjectToExpressionExtensions
{
    /// <summary>
    /// 根据模型生成实体的查询条件
    /// </summary>
    public static Expression<Func<TEntity, bool>>? ToExpression<TEntity, TModel>(this TModel model)
        where TEntity : class
        where TModel : class
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

    /// <summary>
    /// 根据模型生成实体的查询条件
    /// </summary>
    public static string ToPredicate<TEntity, TModel>(this TModel model)
        where TEntity : class
        where TModel : class
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
