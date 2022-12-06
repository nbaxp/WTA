using WTA.Application.Abstractions.Domain;

namespace WTA.Application.Domain.Users;

public class UserLogin : BaseEntity
{
  public string LoginProvider { get; set; } = null!;
  public string ProviderKey { get; set; } = null!;
  public Guid UserId { get; set; }
  public User User { get; set; } = null!;
}
