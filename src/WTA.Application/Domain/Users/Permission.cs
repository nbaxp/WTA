using System.ComponentModel.DataAnnotations;
using WTA.Application.Abstractions.Domain;

namespace WTA.Application.Domain.Users;

[Display(Name = "权限")]
public class Permission : BaseEntity
{
  public string Name { get; set; } = null!;
  public List<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
