using WTA.Application.Abstractions.Domain;

namespace WTA.Application.Domain.Users;

public class UserCenterAttribute : ControllerAttribute
{
  public UserCenterAttribute() : base("Admin", "用户中心") { }
}
