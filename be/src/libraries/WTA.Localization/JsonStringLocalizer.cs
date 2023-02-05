using System.Globalization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace WTA.Localization;
public class JsonStringLocalizer : IStringLocalizer
{
    private string relativeName;
    private bool fallBackToParentCulture;
    private ILogger logger;

    public JsonStringLocalizer(string relativeName, bool fallBackToParentCulture, ILogger logger)
    {
        this.relativeName = relativeName;
        this.fallBackToParentCulture = fallBackToParentCulture;
        this.logger = logger;
    }

    public LocalizedString this[string name]
    {
        get
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            var value = GetStringSafely(name, null);

            return new LocalizedString(name, value ?? name, resourceNotFound: value == null, searchedLocation: _resourceBaseName);
        }
    }

    public LocalizedString this[string name, params object[] arguments]
    {
        get
        {
            var (translation, argumentsWithCount) = GetTranslation(name, arguments);
            var formatted = string.Format(translation.Value, argumentsWithCount);

            return new LocalizedString(name, formatted, translation.ResourceNotFound);
        }
    }


    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
    {
        var culture = CultureInfo.CurrentUICulture;

        return includeParentCultures
            ? GetAllStringsFromCultureHierarchy(culture)
            : GetAllStrings(culture);
    }


}
