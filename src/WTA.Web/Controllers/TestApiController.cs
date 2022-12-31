using Microsoft.AspNetCore.Mvc;
using WTA.Infrastructure.Web.Extensions;
using WTA.Web.Models;

namespace WTA.Web.Controllers;

[Route("[controller]/[action]")]
[ApiController]
public class TestApiController : Controller
{
    [HttpGet]
    public IActionResult Index([FromQuery] TestModel model)
    {
        if (ModelState.IsValid)
        {

        }
        return Json(new
        {
            Model = model,
            Errors = ViewData.ModelState.ToErrors(),
            Schema = model.GetType().GetMetadataForType(this.HttpContext.RequestServices)
        });
    }
}
