namespace WTA.Application.Abstractions.Url;

public interface IUrlService
{
  string GetPath(string url);

  string GetQuery(string url);

  string SetQueryParam(string url, string name, string value);

  void RemoveQueryParam(string url, string name);
}
