using System.Globalization;
using System.Text.Json;
using Microsoft.Extensions.Localization;
using WTA.Resources;

namespace WTA.Infrastructure.Web.Localization;

public class JsonStringLocalizer : IStringLocalizer
{
    public Lazy<Dictionary<string, string>> _keyValuePairs;

    public JsonStringLocalizer()
    {
        _keyValuePairs = new Lazy<Dictionary<string, string>>(() =>
        {
            var assembly = typeof(Resource).Assembly;
            var filePath = $"{assembly.GetName().Name}.Resources.{Thread.CurrentThread.CurrentCulture.Name}.json";
            using var stream = typeof(Resource).Assembly.GetManifestResourceStream(filePath);
            using var jdoc = JsonDocument.Parse(stream);
            var result = jdoc.Deserialize<Dictionary<string, string>>();
            return result;
        });
    }

    public LocalizedString this[string name]
    {
        get
        {
            string value = GetString(name);
            return new LocalizedString(name, value ?? name, value == null);
        }
    }

    public LocalizedString this[string name, params object[] arguments]
    {
        get
        {
            var actualValue = this[name];
            return !actualValue.ResourceNotFound
                ? new LocalizedString(name, string.Format(CultureInfo.InvariantCulture, actualValue.Value, arguments), false)
                : actualValue;
        }
    }

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
    {
        return _keyValuePairs.Value.Select(o => new LocalizedString(o.Key, o.Value));
    }

    private string GetString(string key)
    {
        return _keyValuePairs.Value[key] ?? key;
    }
}
