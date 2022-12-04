using Omu.ValueInjecter;
using WTA.Application.Interfaces;

namespace WTA.Infrastructure.Mapper;

public class DefaultMapper : IMapper
{
  public void From<T>(T to, object from)
  {
    from.InjectFrom(from);
  }

  public T To<T>(object from)
  {
    var target = Activator.CreateInstance<T>();
    target.InjectFrom(from);
    return target!;
  }
}
