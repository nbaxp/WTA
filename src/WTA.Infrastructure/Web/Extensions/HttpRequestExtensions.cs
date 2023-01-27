using Microsoft.AspNetCore.Http;

namespace WTA.Infrastructure.Web.Extensions;

public static class HttpRequestExtensions
{
    public static bool IsJsonRequest(this HttpRequest request)
    {
        return request.Headers.Accept.Contains("application/json");
    }
}
