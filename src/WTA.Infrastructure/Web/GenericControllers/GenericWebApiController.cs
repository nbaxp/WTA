using Microsoft.AspNetCore.Mvc;

namespace WTA.Infrastructure.Web.GenericControllers;

[GenericControllerNameConvention]
[ApiController]
[Route("api/[controller]")]
public class GenericWebApiController<TEntity, TDisplayModel, TEditModel> : ControllerBase
{
  [HttpGet]
  public IActionResult Index()
  {
    return Content($"Hello from a generic {typeof(TEntity).Name} controller.");
  }
}
