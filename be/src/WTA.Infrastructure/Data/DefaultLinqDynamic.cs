using System.Globalization;
using System.Linq.Dynamic.Core;
using System.Reflection;
using Microsoft.OpenApi.Extensions;
using WTA.Application.Abstractions.Application;
using WTA.Application.Abstractions.Components;

namespace WTA.Infrastructure.Data;

[Service<Application.Abstractions.Data.ILinqDynamic>]
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

    public IQueryable<TEntity> Query<TEntity, TModel>(IQueryable<TEntity> source, TModel model)
    {
        var properties = model!.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);
        foreach (var property in properties)
        {
            var propertyName = property.Name;
            var propertyValue = property.GetValue(model, null);
            if (propertyValue != null)
            {
                var attributes = property.GetCustomAttributes<OperatorTypeAttribute>()!;
                var where = attributes.OrderBy(o => o.Order).ToList();
                foreach (var attribute in where)
                {
                    if (typeof(TEntity).GetProperty(propertyName) == null)
                    {
                        continue;
                    }
                    var expression = attribute.OperatorType.GetAttributeOfType<ExpressionAttribute>().Expression;
                    source = source.Where(string.Format(CultureInfo.InvariantCulture, expression, propertyName), propertyValue);
                }
            }
        }
        return source;
    }
}
