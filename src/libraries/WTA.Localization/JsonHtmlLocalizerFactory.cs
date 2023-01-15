using Microsoft.AspNetCore.Mvc.Localization;

namespace WTA.Localization;

public class JsonHtmlLocalizerFactory : IHtmlLocalizerFactory
{
    public IHtmlLocalizer Create(Type resourceSource)
    {
        throw new NotImplementedException();
    }

    public IHtmlLocalizer Create(string baseName, string location)
    {
        throw new NotImplementedException();
    }
}
