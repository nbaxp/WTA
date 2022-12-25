using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace WTA.Infrastructure.Web.GenericControllers;

public class GenericControllerRouteConvention : IControllerModelConvention
{
  public void Apply(ControllerModel controller)
  {
    if (controller.ControllerType.IsGenericType && controller.ControllerType.GetGenericTypeDefinition() == typeof(GenericController<,,>))
    {
      var genericType = controller.ControllerType.GenericTypeArguments[0];
      var routeTemplate = "api/[controller]";
      controller.Selectors.Add(new SelectorModel
      {
        AttributeRouteModel = new AttributeRouteModel(new RouteAttribute(routeTemplate)),
      });
    }
  }
}
