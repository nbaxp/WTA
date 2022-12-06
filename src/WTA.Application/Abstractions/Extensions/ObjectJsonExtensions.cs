using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace WTA.Application.Abstractions.Extensions;

public static class ObjectJsonExtensions
{
  private static readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
  {
    Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
    ReferenceHandler = ReferenceHandler.IgnoreCycles,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
  };

  public static string ToJson(this object value)
  {
    return JsonSerializer.Serialize(value, _jsonSerializerOptions);
  }
}
