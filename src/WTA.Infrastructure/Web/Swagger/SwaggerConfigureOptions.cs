using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WTA.Infrastructure.Web.Swagger;

public class SwaggerConfigureOptions : IConfigureOptions<SwaggerGenOptions>
{
  private readonly IApiDescriptionGroupCollectionProvider provider;

  public SwaggerConfigureOptions(IApiDescriptionGroupCollectionProvider provider)
  {
    this.provider = provider;
  }

  public void Configure(SwaggerGenOptions options)
  {
    foreach (var description in provider.ApiDescriptionGroups.Items)
    {
      options.SwaggerDoc(description.GroupName ?? "default", new OpenApiInfo { Title = description.GroupName ?? "默认分组" });
    }
  }
}
