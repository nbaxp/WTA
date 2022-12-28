using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System.Globalization;
using WTA.Application.Abstractions.Extensions;

namespace WTA.Web.Controllers;

[Route("[controller]/[action]")]
[ApiController]
public class LocalizationController : Controller
{
  private readonly IStringLocalizer _localizer;
  private readonly LinkParser _linkParser;
  private readonly RequestLocalizationOptions _options;

  public LocalizationController(IOptions<RequestLocalizationOptions> options, IStringLocalizer localizer, LinkParser linkParser)
  {
    this._options = options.Value;
    this._localizer = localizer;
    this._linkParser = linkParser;
  }

  [HttpGet]
  public IActionResult Index()
  {
    return View(model: $"""
      当前区域:{Thread.CurrentThread.CurrentCulture.Name}[{Thread.CurrentThread.CurrentCulture.NativeName}],
      localizer["test"]：{this._localizer["test"]}
      """);
  }

  [HttpGet]
  public IActionResult List()
  {
    var options = this._options.SupportedUICultures?
            .Select(o => new { Value = o.Name, Label = o.NativeName })
            .ToList();
    return Json(new
    {
      current = CultureInfo.CurrentCulture.Name,
      options,
    });
  }

  [HttpPost]
  public IActionResult SetLanguage(string culture, string returnUrl)
  {
    var path = returnUrl.GetPath();
    var routes = this._linkParser.ParsePathByAddress("default", path)!;
    var url = $"{Url.RouteUrl("default", routes)}?{returnUrl.GetQuery()}";
    return Json(url);
  }

  [HttpGet]
  public IActionResult Resources()
  {
    return Json(_localizer.GetAllStrings().ToDictionary(o => o.Name, o => o.Value));
  }

  [HttpGet]
  public IActionResult DataAnnotations()
  {
    return View();
  }
}
