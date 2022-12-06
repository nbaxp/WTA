using WTA.Application.Abstractions.Extensions;

namespace WTA.Application.Abstractions.Domain;

public abstract class BaseEntity
{
  public Guid Id { get; set; }

  public BaseEntity()
  {
    Id = AppContext.Current.NewGuid();
  }
}
