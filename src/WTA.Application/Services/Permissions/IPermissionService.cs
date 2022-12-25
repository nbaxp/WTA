namespace WTA.Application.Services.Permissions;

public interface IPermissionService
{
  bool HasPermission(string userName, string permissionNumber);
}
