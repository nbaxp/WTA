using WTA.Application.Abstractions.Domain;

namespace WTA.Application.Domain.Users;

public class RolePermission : BaseEntity, IAssociation
{
  public Guid RoleId { get; set; }
  public Guid PermissionId { get; set; }
  public Role Role { get; set; } = null!;
  public Permission Permission { get; set; } = null!;
}
