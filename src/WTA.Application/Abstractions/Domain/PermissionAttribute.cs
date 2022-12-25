namespace WTA.Application.Abstractions.Domain;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Field, Inherited = false)]
public class PermissionAttribute : Attribute
{
  public PermissionAttribute(OperationType permissions = OperationType.All)
  {
    this.Permissions = permissions;
  }

  public OperationType Permissions { get; }
}
