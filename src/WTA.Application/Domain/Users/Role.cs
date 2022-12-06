using System.ComponentModel.DataAnnotations;
using WTA.Application.Abstractions.Domain;

namespace WTA.Application.Domain.Users;

[UserCenterAttribute]
[Display(Name = "角色")]
public class Role : BaseEntity
{
  [Display(Name = "名称")]
  public string Name { get; set; } = null!;

  public List<UserRole> UserRoles { get; set; } = new List<UserRole>();
  public List<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
