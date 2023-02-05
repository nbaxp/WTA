using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Extensions.Localization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WTA.Infrastructure.Swagger;

internal class SwaggerFilter : IDocumentFilter, IOperationFilter
{
    private readonly IStringLocalizer _localizer;

    public SwaggerFilter(IStringLocalizer localizer)
    {
        _localizer = localizer;
    }

    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var sw = new Stopwatch();
        var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(o => o.FullName.StartsWith("WTA"));
        foreach (var item in swaggerDoc.Components.Schemas.Where(o => o.Key.StartsWith("WTA")))
        {
            var type = assemblies.SelectMany(o => o.GetTypes()).FirstOrDefault(o => o.FullName == item.Key);
            if (item.Value.Description == null)
            {
                item.Value.Description = type?.GetCustomAttribute<DisplayAttribute>()?.Name;
            }
            PropertyInfo? pi = null;
            foreach (var propertyItem in item.Value.Properties)
            {
                if (propertyItem.Value.Description == null)
                {
                    if (type != null)
                    {
                        pi = type?.GetProperties().FirstOrDefault(o => o.Name.Equals(propertyItem.Key, StringComparison.OrdinalIgnoreCase));
                        if (pi != null)
                        {
                            var display = pi?.GetCustomAttribute<DisplayAttribute>();
                            if (display != null)
                            {
                                propertyItem.Value.Description = _localizer.GetString(display.Name!);
                            }
                        }
                    }
                }
            }
        }
        sw.Stop();
        Console.WriteLine($"ms:{sw.ElapsedMilliseconds}");
    }

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {

    }
}
