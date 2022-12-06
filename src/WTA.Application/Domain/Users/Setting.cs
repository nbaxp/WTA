using WTA.Application.Abstractions.Domain;

namespace WTA.Application.Domain.Users;

public class Setting : BaseEntity
{
  public string Key { get; set; } = null!;
  public string Value { get; set; } = null!;
}
