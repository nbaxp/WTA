using Microsoft.AspNetCore.Mvc;

namespace WTA.Infrastructure.Web.GenericControllers;

[GenericControllerNameConvention]
[ApiExplorerSettings(IgnoreApi = true)]
[Route("[controller]/[action]")]
public class GenericWebMvcController<TEntity, TDisplayModel, TEditModel> : Controller
{
  public IActionResult Index()
  {
    return Content($"Hello from a generic {typeof(TEntity).Name} controller.");
  }
}
