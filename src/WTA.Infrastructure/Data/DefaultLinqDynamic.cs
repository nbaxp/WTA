using System.Globalization;
using System.Linq.Dynamic.Core;
using System.Reflection;
using Microsoft.OpenApi.Extensions;
using WTA.Application.Abstractions.Application;

namespace WTA.Infrastructure.Data;

public class DefaultLinqDynamic : Application.Abstractions.Data.ILinqDynamic
{
    public IQueryable<T> Where<T>(IQueryable<T> source, string queryString, params object[] args)
    {
        return source.Where(queryString, args);
    }

    public IQueryable<T> OrderBy<T>(IQueryable<T> source, string ordering, params object[] args)
    {
        return source.OrderBy(ordering, args);
    }

    public IQueryable<TEntity> Where<TEntity, TModel>(IQueryable<TEntity> source, TModel model)
    {
        var properties = model.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);
        foreach (var property in properties)
        {
            var propertyName = property.Name;
            if (typeof(TEntity).GetProperty(propertyName) == null)
            {
                continue;
            }
            var propertyValue = property.GetValue(model, null);
            if (propertyValue != null)
            {
                var attributes = property.GetCustomAttributes<OperatorTypeAttribute>();
                foreach (var attribute in attributes)
                {
                    var expression = attribute.OperatorType.GetAttributeOfType<ExpressionAttribute>().Expression;
                    source = source.Where(string.Format(CultureInfo.InvariantCulture, expression, propertyName), propertyValue);
                }
            }
        }
        return source;
    }
}
