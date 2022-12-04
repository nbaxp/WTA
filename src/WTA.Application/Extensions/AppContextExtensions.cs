using Microsoft.Extensions.DependencyInjection;
using WTA.Application.Interfaces;

namespace WTA.Application.Extensions;

public static class AppContextExtensions
{
  public static Guid NewGuid(this AppContext appContext)
  {
    using var scope = AppContext.Current.Services!.CreateScope();
    return scope.ServiceProvider.GetRequiredService<IGuidGenerator>().Create();
  }
}
