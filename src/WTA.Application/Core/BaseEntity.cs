using WTA.Application.Extensions;

namespace WTA.Application.Core;

public abstract class BaseEntity
{
  public Guid Id { get; set; }

  public BaseEntity()
  {
    Id = AppContext.Current.NewGuid();
  }
}
