using Microsoft.AspNetCore.Mvc;

namespace WTA.Infrastructure.Web.Extensions;

public static class ControllerExtensions
{
  public static bool IsJsonRequest(this ControllerBase controller)
  {
    return controller.Request.Headers.Accept.Contains("application/json");
  }

  public static IActionResult Result(this Controller controller, object? model, string? viewName = null)
  {
    if (controller.IsJsonRequest())
    {
      return controller.Json(new
      {
        model,
        schema = controller.ViewData.ModelMetadata.GetSchema(controller.HttpContext.RequestServices, true)
      });
    }
    return viewName == null ? controller.View(model) : controller.View(viewName, model);
  }
}
