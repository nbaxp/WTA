using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using WTA.Application.Abstractions.Domain;

namespace WTA.Infrastructure.Web.GenericControllers;

public class GenericControllerRouteConvention : IControllerModelConvention
{
  public void Apply(ControllerModel controller)
  {
    if (controller.ControllerType.IsGenericType && controller.ControllerType.GetGenericTypeDefinition() == typeof(GenericWebController<,,>))
    {
      var genericType = controller.ControllerType.GenericTypeArguments[0];
      var attribute = genericType.GetCustomAttributes(true).FirstOrDefault(o => o.GetType().IsAssignableTo(typeof(MetaAttribute)));
      var routeTemplate = "api/[controller]";
      if (attribute != null)
      {
        var group = (attribute as MetaAttribute)?.Group;
        if (!string.IsNullOrEmpty(group))
        {
          routeTemplate = $"api/{group}/[controller]";
          //controller.ApiExplorer.GroupName = group;
        }
        controller.Selectors.Add(new SelectorModel
        {
          AttributeRouteModel = new AttributeRouteModel(new RouteAttribute(routeTemplate)),
        });
      }
    }
  }
}
