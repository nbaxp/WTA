using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Globalization;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using WTA.Application.Abstractions;
using WTA.Application.Abstractions.Data;
using WTA.Application.Domain.Users;
using WTA.Application.Services;
using WTA.Infrastructure.Data;
using WTA.Infrastructure.EventBus;
using WTA.Infrastructure.Mapper;
using WTA.Infrastructure.Resources;
using WTA.Infrastructure.Services;
using WTA.Infrastructure.Web.GenericControllers;
using WTA.Infrastructure.Web.Routing;
using WTA.Infrastructure.Web.Swagger;

namespace WTA.Infrastructure.Web.Extensions;

public static class WebApplicationBuilderExtensions
{
  public static void Config(this WebApplicationBuilder builder)
  {
    builder.Services.AddWebEncoders(options => options.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All));
    builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
    {
      options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
      options.SerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
      options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });
    builder.Services.Configure<FormOptions>(options =>
    {
      options.ValueCountLimit = int.MaxValue;
      options.MultipartBodyLengthLimit = long.MaxValue;
    });
    builder.Services.AddPortableObjectLocalization(options => options.ResourcesPath = "Resources");//po
    AddServices(builder);
    AddMvc(builder);
    AddLocalization(builder);
    AddSwagger(builder);
    AddDbContext(builder);
  }

  private static void AddServices(WebApplicationBuilder builder)
  {
    builder.Services.AddHttpClient();
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, SwaggerConfigureOptions>();
    builder.Services.AddEventBus();
    builder.Services.AddSingleton<ILinqInclude, DefaultLinqInclude>();
    builder.Services.AddSingleton<ILinqDynamic, DefaultLinqDynamic>();
    builder.Services.AddSingleton<IMapper, DefaultMapper>();
    builder.Services.AddTransient<IGuidGenerator, DefaultGuidGenerator>();
    builder.Services.AddScoped<ITenantService, TenantService>();
    builder.Services.AddTransient<IPasswordHasher, PasswordHasher>();
    builder.Services.AddTransient<ITestService<User>, TestService>();
  }

  private static void AddMvc(WebApplicationBuilder builder)
  {
    builder.Services.AddRouting(options => options.ConstraintMap["slugify"] = typeof(SlugifyParameterTransformer));
    // MVC
    builder.Services.AddMvc(options =>
    {
      options.Conventions.Add(new GenericControllerRouteConvention());
      //options.ModelMetadataDetailsProviders.Insert(0, new CustomIDisplayMetadataProvider());
      // 小写 + 连字符格式
      options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer()));
    })
    .AddDataAnnotationsLocalization(options =>
    {
      options.DataAnnotationLocalizerProvider = (type, factory) =>
      {
        var localizer = factory.Create(typeof(Resource));
        return localizer;
      };
    })
    .ConfigureApplicationPartManager(o => o.FeatureProviders.Add(new GenericControllerFeatureProvider()))
    .AddControllersAsServices();// must after ConfigureApplicationPartManager
  }

  private static void AddLocalization(WebApplicationBuilder builder)
  {
    builder.Services.AddPortableObjectLocalization();

    builder.Services.Configure<RequestLocalizationOptions>(options =>
    {
      var supportedCultures = new List<CultureInfo>
      {
        new CultureInfo("zh-Hans-CN"),
        new CultureInfo("en-US")
      };

      options.DefaultRequestCulture = new RequestCulture(supportedCultures.First());
      options.SupportedCultures = supportedCultures;
      options.SupportedUICultures = supportedCultures;
    });
  }

  private static void AddSwagger(WebApplicationBuilder builder)
  {
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
  }

  private static void AddDbContext(WebApplicationBuilder builder)
  {
    builder.Services.AddScoped<DbContext, AppDbContext>();
    builder.Services.AddScoped<AppDbContextSeed>();
    builder.Services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
    var dbKey = builder.Configuration.GetConnectionString("Default") ?? "sqlite";
    var connectionString = builder.Configuration.GetConnectionString(dbKey);
    builder.Services.AddPooledDbContextFactory<AppDbContext>(
        options =>
        {
          if (dbKey == "mysql")
          {
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
          }
          else
          {
            options.UseSqlite(connectionString);
          }
        });
  }
}
