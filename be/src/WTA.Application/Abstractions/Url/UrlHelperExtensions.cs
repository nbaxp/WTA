using Microsoft.Extensions.DependencyInjection;
using WTA.Application.Abstractions.Url;

namespace WTA.Application.Abstractions.Extensions;

public static class UrlHelperExtensions
{
    public static string GetPath(this string url)
    {
        using var scope = App.Current.Services!.CreateScope();
        var helper = scope.ServiceProvider.GetRequiredService<IUrlService>();
        return helper.GetPath(url);
    }

    public static string GetQuery(this string url)
    {
        using var scope = App.Current.Services!.CreateScope();
        var helper = scope.ServiceProvider.GetRequiredService<IUrlService>();
        return helper.GetQuery(url);
    }
}
