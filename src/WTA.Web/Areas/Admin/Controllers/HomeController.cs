using Microsoft.AspNetCore.Mvc;

namespace WTA.Web.Areas.Admin;

[Area("Admin")]
public class HomeController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }
}
