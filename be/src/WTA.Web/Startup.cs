using WTA.Application.Abstractions.Extensions;
using WTA.Application.Services;
using WTA.Infrastructure.EventBus;
using WTA.Infrastructure.Mvc;

namespace WTA.Web;

public class Startup : BaseStartup
{
    public override void ConfigureServices(WebApplicationBuilder builder)
    {
        base.ConfigureServices(builder);
        builder.Services.AddDefaultServices(o => o.FullName!.StartsWith(nameof(WTA)));
        builder.Services.AddDefaultOptions(builder.Configuration, o => o.FullName!.StartsWith(nameof(WTA)));
        builder.Services.AddEventBus(o => o.FullName!.StartsWith(nameof(WTA)));
        builder.Services.AddTransient(typeof(IApplicationService<>), typeof(ApplicationService<>));
    }
}
