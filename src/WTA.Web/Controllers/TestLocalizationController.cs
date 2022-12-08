using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace WTA.Web.Controllers;

[ApiExplorerSettings(GroupName = "Test")]
//[Route("{lang=zh-cn}/[controller]/[action]")]
[Route("[controller]/[action]")]
[ApiController]
public class TestLocalizationController : Controller
{
  private readonly IStringLocalizer _localizer;
  private readonly RequestLocalizationOptions _options;

  public TestLocalizationController(IOptions<RequestLocalizationOptions> options, IStringLocalizer localizer)
  {
    this._options = options.Value;
    this._localizer = localizer;
  }

  [HttpGet]
  public IActionResult Index()
  {
    return Content($"""
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
    Response.Cookies.Append(
        CookieRequestCultureProvider.DefaultCookieName,
        CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
        new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
    );
    return LocalRedirect(returnUrl);
  }
}
