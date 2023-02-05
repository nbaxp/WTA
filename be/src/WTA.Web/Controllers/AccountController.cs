using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using WTA.Application.Services.Users;
using WTA.Infrastructure.Extensions;
using WTA.Infrastructure.Mvc;

namespace WTA.Web.Controllers;

[Authorize]
[Route("[controller]/[action]")]
public class AccountController : Controller
{
    private readonly IStringLocalizer _localizer;
    private readonly IUserService _userService;
    private readonly ITokenService _tokenService;

    public AccountController(IStringLocalizer localizer, IUserService userService, ITokenService tokenService)
    {
        this._localizer = localizer;
        this._userService = userService;
        this._tokenService = tokenService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login(string returnUrl)
    {
        var model = new LoginModel { ReturnUrl = returnUrl };
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
                    if (this.Request.IsJsonRequest())
                    {
                        var tokenResult = this._tokenService.CreateAuth2TokenResult(model.UserName, model.RememberMe);
                        return Json(tokenResult, new JsonSerializerOptions { PropertyNamingPolicy = new UnderlineJsonNamingPolicy() });
                    }
                    else
                    {
                        var key = UnderlineJsonNamingPolicy.ToUnderline(nameof(OAuth2TokenResult.AccessToken));
                        var accessTokenForCookie = this._tokenService.CreatAccessTokenForCookie(model.UserName, model.RememberMe, out var timeout);
                        var cookieOptions = new CookieOptions
                        {
                            HttpOnly = true,
                            Expires = DateTimeOffset.Now.Add(timeout)
                        };
                        if (Request.Cookies.Keys.Contains(key))
                        {
                            Response.Cookies.Delete(key);
                        }
                        Response.Cookies.Append(key, accessTokenForCookie);
                        if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                        {
                            return Redirect(model.ReturnUrl);
                        }
                        else
                        {
                            return RedirectToAction(null);
                        }
                    }
                }
                ModelState.AddModelError("", _localizer[result.Status.ToString()]);
                return this.Result(model);
            }
            return BadRequest(ModelState.ToErrors());
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    [HttpPost]
    public IActionResult Logout()
    {
        var key = UnderlineJsonNamingPolicy.ToUnderline(nameof(OAuth2TokenResult.AccessToken));
        Response.Cookies.Delete(key);
        return Ok(true);
    }
}
