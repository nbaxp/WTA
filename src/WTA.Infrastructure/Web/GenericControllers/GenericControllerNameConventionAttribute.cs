using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace WTA.Infrastructure.Web.GenericControllers;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class GenericControllerNameConvention : Attribute, IControllerModelConvention
{
  public void Apply(ControllerModel controller)
  {
    if (controller.ControllerType.GetGenericTypeDefinition() == typeof(GenericWebController<,,>))
    {
      var entityType = controller.ControllerType.GenericTypeArguments[0];
      controller.ControllerName = entityType.Name;
    }
  }
}
