using Microsoft.Extensions.Caching.Distributed;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace WTA.Application.Abstractions.Extensions;

public static class DistributedCacheExtensions
{
  private static readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
  {
    Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
    ReferenceHandler = ReferenceHandler.IgnoreCycles,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
  };

  public static T? GetItem<T>(this IDistributedCache cache, string key)
  {
    return JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(cache.Get(key) ?? Array.Empty<byte>()), _jsonSerializerOptions);
  }

  public static void SetItem<T>(this IDistributedCache cache, string key, T value)
  {
    cache.Set(key, Encoding.UTF8.GetBytes(JsonSerializer.Serialize(value, _jsonSerializerOptions)));
  }
}
