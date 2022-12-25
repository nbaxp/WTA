using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using WTA.Application.Abstractions.Data;

namespace WTA.Infrastructure.Web.Extensions;

public static class WebApplicationExtensions
{
  public static void Configure(this WebApplication app)
  {
    Application.AppContext.Configure(app.Services);
    if (app.Environment.IsDevelopment())
    {
      app.UseExceptionHandler("/Error");
    }
    UseStaticFiles(app);
    UseRouting(app);
    UseLocalization(app);
    UseSwagger(app);
    UseAuthorization(app);
    UseDatabase(app);
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
    app.MapControllerRoute(name: "default", pattern: "{culture:slugify=zh}/{controller:slugify=Home}/{action:slugify=Index}/{id?}");
  }

  private static void UseLocalization(WebApplication app)
  {
    var localizationOptions = app.Services.GetService<IOptions<RequestLocalizationOptions>>()!.Value;
    app.UseRequestLocalization(localizationOptions);
  }

  private static void UseSwagger(WebApplication app)
  {
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
      var apiDescriptionGroups = app.Services.GetRequiredService<IApiDescriptionGroupCollectionProvider>().ApiDescriptionGroups.Items;
      foreach (var description in apiDescriptionGroups)
      {
        if (description.GroupName is not null)
        {
          options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName);
        }
        else
        {
          options.SwaggerEndpoint($"/swagger/default/swagger.json", "Default");
        }
      }
    });
  }

  private static void UseAuthorization(WebApplication app)
  {
    app.UseCors();
    app.UseAuthentication();
    app.UseAuthorization();
  }

  private static void UseDatabase(WebApplication app)
  {
    using var scope = app.Services.CreateScope();
    using var db = scope.ServiceProvider.GetRequiredService<DbContext>();
    if (db.Database.EnsureCreated())
    {
      scope.ServiceProvider.GetRequiredService<IDbSeed>().Seed().Wait();
    }
  }
}
