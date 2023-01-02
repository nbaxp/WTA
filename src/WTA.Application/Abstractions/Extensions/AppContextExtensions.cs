using Microsoft.Extensions.DependencyInjection;

namespace WTA.Application.Abstractions.Extensions;

public static class AppContextExtensions
{
    public static Guid NewGuid(this ApplicationContext appContext)
    {
        using var scope = ApplicationContext.Current.Services!.CreateScope();
        return scope.ServiceProvider.GetRequiredService<IGuidGenerator>().Create();
    }
}
