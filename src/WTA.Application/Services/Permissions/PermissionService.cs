using Microsoft.Extensions.Caching.Distributed;
using WTA.Application.Abstractions.Data;
using WTA.Application.Domain.System;

namespace WTA.Application.Services.Permissions;

public class PermissionService : IPermissionService
{
  public const string CACHE_KEY = nameof(Permission);
  private readonly IRepository<User> _userRepository;
  private readonly IRepository<Permission> _permissionRepository;
  private readonly IDistributedCache _distributedCache;

  public PermissionService(IRepository<User> userRepository, IRepository<Permission> permissionRepository, IDistributedCache distributedCache)
  {
    this._userRepository = userRepository;
    this._permissionRepository = permissionRepository;
    this._distributedCache = distributedCache;
  }

  //public List<Permission> GetItems()
  //{
  //  this._distributedCache.get
  //}

  public bool HasPermission(string userName, string permissionNumber)
  {
    return this._userRepository.AsNoTracking()
      .Where(o => o.UserName == userName)
      .Any(o => o.UserRoles.Any(o => o.Role.RolePermissions.Any(rp => rp.Permission.Number == permissionNumber))); ;
  }
}
