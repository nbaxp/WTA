using Microsoft.AspNetCore.Routing;
using System.Text.RegularExpressions;

namespace WTA.Infrastructure.Web.Routing;

public class SlugifyParameterTransformer : IOutboundParameterTransformer
{
  public string TransformOutbound(object? value)
  {
    return Regex.Replace(value?.ToString()!, "([a-z])([A-Z])", "$1-$2").ToLowerInvariant();
  }
}
