using Flurl;
using WTA.Application.Abstractions.Components;
using WTA.Application.Abstractions.Url;

namespace WTA.Infrastructure.Urls;

[Service<IUrlService>]
public class DefaultUrlService : IUrlService
{
    public string GetPath(string url)
    {
        return Url.Parse(url).Path;
    }

    public string GetQuery(string url)
    {
        return Url.Parse(url).Query;
    }

    public string SetQueryParam(string url, string name, string value)
    {
        return url.SetQueryParam(name, value);
    }

    public void RemoveQueryParam(string url, string name)
    {
        url.RemoveQueryParam(name);
    }
}
