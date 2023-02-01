using System.Text.Encodings.Web;
using System.Text.Unicode;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace WTA.Infrastructure.Web;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddWebEncoders(options => options.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All));

    }

    public void Configure(IApplicationBuilder app)
    {
    }
}
