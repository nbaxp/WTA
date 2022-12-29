using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection;
using WTA.Application.Services.Permissions;

namespace WTA.Infrastructure.Web.Authentication;

public class CustomClaimsPrincipal : ClaimsPrincipal
{
    private readonly IServiceProvider _serviceProvider;

    public CustomClaimsPrincipal(IServiceProvider serviceProvider, ClaimsPrincipal claimsPrincipal) : base(claimsPrincipal)
    {
        this._serviceProvider = serviceProvider;
    }

    public override bool IsInRole(string role)
    {
        using var scope = this._serviceProvider.CreateScope();
        var permissionService = scope.ServiceProvider.GetRequiredService<IPermissionService>();
        return permissionService.HasPermission(this.Identity!.Name!, role);
    }
}
