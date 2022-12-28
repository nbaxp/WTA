using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WTA.Application.Authentication;
using WTA.Application.Services.Users;

namespace WTA.Web.Controllers;

[Route("[controller]/[action]")]
[ApiController]
public class TokenController : Controller
{
  private readonly string TOKEN_COOKIE_NAME = "refresh_token";
  private readonly TokenValidationParameters _tokenValidationParameters;
  private readonly IUserService _userService;
  private readonly IStringLocalizer _localizer;
  private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;
  private readonly SigningCredentials _credentials;
  private readonly JwtOptions _jwtOptions;
  private readonly IHttpContextAccessor _httpContextAccessor;

  public TokenController(TokenValidationParameters tokenValidationParameters,
    IUserService userService,
    IStringLocalizer localizer,
    JwtSecurityTokenHandler jwtSecurityTokenHandler,
    SigningCredentials credentials,
    IOptions<JwtOptions> jwtOptions,
    IHttpContextAccessor httpContextAccessor)
  {
    this._tokenValidationParameters = tokenValidationParameters;
    this._userService = userService;
    this._localizer = localizer;
    this._jwtSecurityTokenHandler = jwtSecurityTokenHandler;
    this._credentials = credentials;
    this._jwtOptions = jwtOptions.Value;
    this._httpContextAccessor = httpContextAccessor;
  }

  [HttpPost]
  public IActionResult Create([FromBody] LoginModel model)
  {
    if (ModelState.IsValid)
    {
      var result = _userService.ValidateUser(model);
      if (result.Status == ValidateUserStatus.Successful)
      {
        var user = result.User!;
        var claims = new Claim[] {
            new Claim(this._tokenValidationParameters.NameClaimType, user.UserName!),
            new Claim(nameof(user.Name), user.Name!),
            new Claim(nameof(user.RoleHash),user.RoleHash!)
        };
        var roleClaims = _userService.GetRoles(model.UserName)
          .Select(o => new Claim(this._tokenValidationParameters.RoleClaimType, o.Name));
        var now = DateTime.UtcNow;
        var accessToken = CreateToken(new ClaimsIdentity(claims.Concat(roleClaims), JwtBearerDefaults.AuthenticationScheme), now, _jwtOptions.AccessTokenExpires);
        var refreshToken = CreateToken(new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme), now, _jwtOptions.RefreshTokenExpires);
        SetRefreshCookie(refreshToken);
        return this.Ok(accessToken);
      }
      ModelState.AddModelError("", _localizer[result.Status.ToString()]);
    }
    return Problem();
  }

  [HttpPost]
  public IActionResult Refresh()
  {
    try
    {
      var refreshToken = _httpContextAccessor.HttpContext?.Request.Cookies[TOKEN_COOKIE_NAME];
      var claimsPrincipal = this._jwtSecurityTokenHandler.ValidateToken(refreshToken, _tokenValidationParameters, out _);
      var claimsIdentity = new ClaimsIdentity(claimsPrincipal.Identity);
      var now = DateTime.UtcNow;
      var accessToken = CreateToken(claimsIdentity, now, _jwtOptions.AccessTokenExpires);
      var newRefreshToken = CreateToken(claimsIdentity, now, _jwtOptions.RefreshTokenExpires);
      SetRefreshCookie(newRefreshToken);
      return Ok(accessToken);
    }
    catch (Exception ex)
    {
      return Problem(ex.Message);
    }
  }

  private string CreateToken(ClaimsIdentity subject, DateTime now, TimeSpan timeout)
  {
    var tokenDescriptor = new SecurityTokenDescriptor()
    {
      Issuer = this._jwtOptions.Issuer,
      Audience = this._jwtOptions.Audience,
      SigningCredentials = _credentials,
      Subject = subject,
      IssuedAt = now,
      NotBefore = now,
      Expires = now.Add(timeout),
    };
    var securityToken = this._jwtSecurityTokenHandler.CreateJwtSecurityToken(tokenDescriptor);
    var token = this._jwtSecurityTokenHandler.WriteToken(securityToken);
    return token;
  }

  private void SetRefreshCookie(string refreshToken)
  {
    var cookieOptions = new CookieOptions
    {
      HttpOnly = true,
      Path = Url.Action(nameof(Refresh))!,
      Expires = DateTimeOffset.Now.Add(_jwtOptions.RefreshTokenExpires)
    };
    _httpContextAccessor.HttpContext?.Response.Cookies.Delete(TOKEN_COOKIE_NAME, cookieOptions);
    _httpContextAccessor.HttpContext?.Response.Cookies.Append(TOKEN_COOKIE_NAME, refreshToken, cookieOptions);
  }
}
