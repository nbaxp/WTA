using System.Security.Claims;

namespace WTA.Application.Services.Users;

public interface ITokenService
{
    OAuth2TokenResult CreateToken(string userName, bool rememberMe, params Claim[] additionalClaims);
}
