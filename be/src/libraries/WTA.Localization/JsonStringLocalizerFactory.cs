using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace WTA.Localization;

public class JsonStringLocalizerFactory : IStringLocalizerFactory
{
    private readonly bool _fallBackToParentCulture;
    private readonly ILogger _logger;

    /// <summary>
    /// Creates a new instance of <see cref="PortableObjectStringLocalizerFactory"/>.
    /// </summary>
    /// <param name="localizationManager"></param>
    /// <param name="requestLocalizationOptions"></param>
    /// <param name="logger"></param>
    public JsonStringLocalizerFactory(
        IOptions<RequestLocalizationOptions> requestLocalizationOptions,
        ILogger<JsonStringLocalizerFactory> logger)
    {
        _fallBackToParentCulture = requestLocalizationOptions.Value.FallBackToParentUICultures;
        _logger = logger;
    }

    public IStringLocalizer Create(Type resourceSource)
    {
        var resourceFullName = resourceSource.FullName;
        resourceFullName = TryFixInnerClassPath(resourceFullName);

        return new JsonStringLocalizer(resourceFullName, _fallBackToParentCulture, _logger);
    }

    public IStringLocalizer Create(string baseName, string location)
    {
        baseName = TryFixInnerClassPath(baseName);

        var index = 0;
        if (baseName.StartsWith(location, StringComparison.OrdinalIgnoreCase))
        {
            index = location.Length;
        }

        if (baseName.Length > index && baseName[index] == '.')
        {
            index += 1;
        }

        if (baseName.Length > index && baseName.IndexOf("Areas.", index, StringComparison.Ordinal) == index)
        {
            index += "Areas.".Length;
        }

        var relativeName = baseName.Substring(index);

        return new JsonStringLocalizer(relativeName, _fallBackToParentCulture, _logger);
    }

    private string TryFixInnerClassPath(string context)
    {
        const char innerClassSeparator = '+';
        var path = context;
        if (context.Contains(innerClassSeparator))
        {
            path = context.Replace(innerClassSeparator, '.');
        }

        return path;
    }
}
