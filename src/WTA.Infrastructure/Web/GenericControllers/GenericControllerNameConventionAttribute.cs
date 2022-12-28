using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System.Reflection;
using WTA.Application.Abstractions.Domain;

namespace WTA.Infrastructure.Web.GenericControllers;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class GenericControllerNameConventionAttribute : Attribute, IControllerModelConvention
{
  public void Apply(ControllerModel controller)
  {
    if (controller.ControllerType.GetGenericTypeDefinition() != typeof(GenericController<,,>))
    {
      return;
    }
    var entityType = controller.ControllerType.GenericTypeArguments[0];
    controller.ControllerName = entityType.Name;
    var groupName = entityType.GetCustomAttribute<GroupAttribute>()?.Area;
    if (!string.IsNullOrEmpty(groupName))
    {
      controller.ApiExplorer.GroupName = groupName;
    }
  }
}
