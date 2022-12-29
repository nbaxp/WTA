using Microsoft.AspNetCore.Mvc;
using WTA.Infrastructure.Web.Extensions;
using WTA.Web.Models;

namespace WTA.Web.Controllers;

//[Route("api/[controller]/[action]")]
//[ApiController]
public class TestApiController : Controller
{
    [HttpGet]
    public IActionResult Index(TestModel model)
    {
        return Json(new
        {
            Model = model,
            Errors = ViewData.ModelState.ToErrors(),
            Schema = ViewData.ModelMetadata.GetSchema(this.HttpContext.RequestServices)
        });
    }
}
