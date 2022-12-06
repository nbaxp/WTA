using Microsoft.Extensions.DependencyInjection;

namespace WTA.Application.Abstractions.Extensions;

public static class AppContextExtensions
{
  public static Guid NewGuid(this AppContext appContext)
  {
    using var scope = AppContext.Current.Services!.CreateScope();
    return scope.ServiceProvider.GetRequiredService<IGuidGenerator>().Create();
  }
}
