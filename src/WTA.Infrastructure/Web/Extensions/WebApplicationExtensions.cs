using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WTA.Infrastructure.Data;

namespace WTA.Infrastructure.Web.Extensions;

public static class WebApplicationExtensions
{
  public static void Configure(this WebApplication app)
  {
    Application.AppContext.Configure(app.Services);
    if (app.Environment.IsDevelopment())
    {
      app.UseExceptionHandler("/Error");
      app.UseSwagger();
      app.UseSwaggerUI();
    }
    UseStaticFiles(app);
    UseRouting(app);
    UseDatabase(app);
    app.UseAuthorization();
  }

  private static void UseStaticFiles(WebApplication app)
  {
    // 设置首页为 index.html
    var options = new DefaultFilesOptions();
    options.DefaultFileNames.Clear();
    options.DefaultFileNames.Add("index.html");
    app.UseDefaultFiles(options);

    // app 下载配置
    var provider = new FileExtensionContentTypeProvider();
    provider.Mappings.Add(".apk", "application/vnd.android.package-archive");
    provider.Mappings.Add(".plist", "text/xml");
    provider.Mappings.Add(".ipa", "application/vnd.android.package-archive");
    app.UseStaticFiles(new StaticFileOptions
    {
      ContentTypeProvider = provider,
      ServeUnknownFileTypes = true,
      DefaultContentType = "application/octet-stream"
    });
  }

  private static void UseRouting(WebApplication app)
  {
    app.UseRouting();
    app.UseEndpoints(endpoints =>
    {
      //var requestLocalizationOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value;
      //var defaults = new { culture = requestLocalizationOptions.DefaultRequestCulture.Culture.Name };

      //app.MapControllerRoute(
      //    name: "area",
      //    pattern: "{area:exists:slugify}/{controller:slugify=Home}/{action:slugify=Index}/{id?}", defaults: defaults);

      //app.MapControllerRoute(
      //    name: "default",
      //    pattern: "{controller:slugify=Home}/{action:slugify=Index}/{id?}", defaults: defaults);

      //endpoints.MapDynamicControllerRoute<TestTransformer>("{culture:slugify=zh-Hans}/{controller:slugify=Home}/{action:slugify=Index}");

      // endpoints.MapSwagger();
      // endpoints.MapHub<TestHub>("/hub");
      app.MapControllerRoute(name: "default", pattern: "{controller:slugify=Home}/{action:slugify=Index}/{id?}");
    });
  }

  private static void UseDatabase(WebApplication app)
  {
    using var scope = app.Services.CreateScope();
    using var db = scope.ServiceProvider.GetRequiredService<DbContext>();
    if (db.Database.EnsureCreated())
    {
      scope.ServiceProvider.GetRequiredService<AppDbContextSeed>().Seed().Wait();
    }
  }
}
