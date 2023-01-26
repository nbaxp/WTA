using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WTA.Application.Authentication;
using WTA.Application.Services.Users;

namespace WTA.Infrastructure.Web.Authentication;

public class TokenService : ITokenService
{
    private readonly TokenValidationParameters _tokenValidationParameters;
    private readonly JwtOptions _jwtOptions;
    private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;
    private readonly SigningCredentials _credentials;

    public TokenService(TokenValidationParameters tokenValidationParameters,
        JwtSecurityTokenHandler jwtSecurityTokenHandler,
        SigningCredentials credentials,
        IOptions<JwtOptions> jwtOptions)
    {
        this._tokenValidationParameters = tokenValidationParameters;
        this._jwtSecurityTokenHandler = jwtSecurityTokenHandler;
        this._credentials = credentials;
        this._jwtOptions = jwtOptions.Value;
    }

    public OAuth2TokenResult CreateToken(string userName, bool rememberMe, params Claim[] additionalClaims)
    {
        var now = DateTime.UtcNow;
        var accessTokenTimeout = rememberMe ? TimeSpan.FromDays(365) : _jwtOptions.AccessTokenExpires;
        var claims = new List<Claim>(additionalClaims){
            new Claim(_tokenValidationParameters.NameClaimType,userName)
        };
        var subject = new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme);
        return new OAuth2TokenResult
        {
            AccessToken = CreateToken(subject, now, accessTokenTimeout),
            RefreshToken = CreateToken(subject, now, _jwtOptions.RefreshTokenExpires),
            expiresIn = (long)accessTokenTimeout.TotalSeconds
        };
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
}
