using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;
using WTA.Localization;

namespace Microsoft.Extensions.DependencyInjection;

public static class LocalizationServiceCollectionExtensions
{
    public static IServiceCollection AddJsonLocalization(this IServiceCollection services)
    {
        return services.AddJsonLocalization(null);
    }

    public static IServiceCollection AddJsonLocalization(this IServiceCollection services, Action<LocalizationOptions> setupAction)
    {
        services.AddSingleton<IStringLocalizerFactory, JsonStringLocalizerFactory>();
        services.AddSingleton<IHtmlLocalizerFactory, JsonHtmlLocalizerFactory>();
        services.TryAddTransient(typeof(IStringLocalizer<>), typeof(StringLocalizer<>));
        if (setupAction != null)
        {
            services.Configure(setupAction);
        }

        return services;
    }
}
