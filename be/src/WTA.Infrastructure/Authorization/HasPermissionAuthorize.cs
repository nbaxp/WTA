using Microsoft.AspNetCore.Authorization;

namespace WTA.Infrastructure.Authorization;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = false)]
public class HasPermissionAttribute : AuthorizeAttribute
{
    public HasPermissionAttribute(Enum permission) : base(permission.ToString())
    {
    }
}
