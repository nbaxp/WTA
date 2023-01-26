using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using WTA.Application.Services.Users;
using WTA.Infrastructure.Web.Extensions;
using WTA.Infrastructure.Web.Mvc;

namespace WTA.Web.Controllers;

[Authorize]
[Route("[controller]/[action]")]
public class AccountController : Controller
{
    private readonly IStringLocalizer _localizer;
    private readonly IUserService _userService;

    public AccountController(IStringLocalizer localizer, IUserService userService)
    {
        this._localizer = localizer;
        this._userService = userService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login()
    {
        var model = new LoginModel();
        return this.Result(model);
    }

    [AllowAnonymous]
    [HttpPost]
    public IActionResult Login([FromBody] LoginModel model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = _userService.ValidateUser(model);
                if (result.Status == ValidateUserStatus.Successful)
                {
                    return Json(result.TokenResult, new JsonSerializerOptions { PropertyNamingPolicy = new UnderlineJsonNamingPolicy() });
                }
                ModelState.AddModelError("", _localizer[result.Status.ToString()]);
                return this.Result(model);
            }
            return BadRequest();
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }
}
