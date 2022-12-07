using WTA.Application.Abstractions.Domain;

namespace WTA.Application.Domain.Users;

public class UserCenterAttribute : MetaAttribute
{
  public UserCenterAttribute() : base("uc")
  {
  }
}
