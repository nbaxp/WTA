using Microsoft.AspNetCore.Mvc;

namespace WTA.Web.Controllers;

public class ConfigurationController : Controller
{
    private readonly IConfiguration _cfg;

    public ConfigurationController(IConfiguration cfg)
    {
        this._cfg = cfg;
    }

    public IActionResult Index()
    {
        return Json(_cfg);
    }
}
