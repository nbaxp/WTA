using Microsoft.AspNetCore.Http;
using WTA.Application.Abstractions;
using WTA.Application.Abstractions.Components;

namespace WTA.Infrastructure.Services;

[Service<ITenantService>]
public class TenantService : ITenantService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private string? _tenant;

    public TenantService(IHttpContextAccessor httpContextAccessor)
    {
        this._httpContextAccessor = httpContextAccessor;
    }

    public string? Tenant
    {
        get
        {
            if (_tenant == null)
            {
                this._tenant = this._httpContextAccessor.HttpContext?.Request.Host.Host;
            }
            return _tenant;
        }
    }
}
