using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using WTA.Application.Abstractions;
using WTA.Application.Abstractions.Data;
using WTA.Application.Abstractions.Url;
using WTA.Application.Authentication;
using WTA.Application.Domain.System;
using WTA.Application.Services;
using WTA.Application.Services.Permissions;
using WTA.Application.Services.Users;
using WTA.Infrastructure.Data;
using WTA.Infrastructure.EventBus;
using WTA.Infrastructure.Mapper;
using WTA.Infrastructure.Resources;
using WTA.Infrastructure.Services;
using WTA.Infrastructure.Uri;
using WTA.Infrastructure.Web.Authentication;
using WTA.Infrastructure.Web.GenericControllers;
using WTA.Infrastructure.Web.ModelBinding;
using WTA.Infrastructure.Web.Routing;
using WTA.Infrastructure.Web.Swagger;

namespace WTA.Infrastructure.Web.Extensions;

public static class WebApplicationBuilderExtensions
{
  public static void BuildAndRun(this WebApplicationBuilder builder)
  {
    var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(path: "appsettings.json", optional: false, reloadOnChange: true)
                .Build();
    Log.Logger = new LoggerConfiguration()
      .ReadFrom.Configuration(configuration)
      .CreateBootstrapLogger();
    try
    {
      Log.Information("Starting web application");
      builder.Host.UseSerilog((hostingContext, configBuilder) =>
      {
        configBuilder.ReadFrom.Configuration(hostingContext.Configuration);
      });
      builder.Configure();
      var app = builder.Build();
      app.UseSerilogRequestLogging();
      app.Configure();
      app.Run();
    }
    catch (Exception ex)
    {
      Log.Fatal(ex, "Application terminated unexpectedly");
    }
    finally
    {
      Log.Information("web application closed");
      Log.CloseAndFlush();
    }
  }

  public static void Configure(this WebApplicationBuilder builder)
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
    AddServices(builder);
    AddLocalization(builder);
    AddMvc(builder);
    AddSwagger(builder);
    AddAuthentication(builder);
    AddCache(builder);
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
    builder.Services.AddSingleton<IUrlService, DefaultUrlService>();
    builder.Services.AddTransient<IGuidGenerator, DefaultGuidGenerator>();
    builder.Services.AddScoped<ITenantService, TenantService>();
    builder.Services.AddTransient<IPasswordHasher, PasswordHasher>();

    builder.Services.Configure<IdentityOptions>(builder.Configuration.GetSection(IdentityOptions.Position));
    builder.Services.TryAddTransient<IUserService, UserService>();
    builder.Services.TryAddTransient<IPermissionService, PermissionService>();
    builder.Services.AddTransient<ITestService<User>, TestService>();
  }

  private static void AddAuthentication(WebApplicationBuilder builder)
  {
    builder.Services.AddSingleton<CustomJwtSecurityTokenHandler>();
    builder.Services.AddSingleton<JwtSecurityTokenHandler, CustomJwtSecurityTokenHandler>();
    builder.Services.AddSingleton<IPostConfigureOptions<JwtBearerOptions>, CustomJwtBearerPostConfigureOptions>();
    builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.Position));
    var jwtOptions = new JwtOptions();
    builder.Configuration.GetSection(JwtOptions.Position).Bind(jwtOptions);
    var issuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key));
    builder.Services.AddSingleton(new SigningCredentials(issuerSigningKey, SecurityAlgorithms.HmacSha256Signature));
    var tokenValidationParameters = new TokenValidationParameters
    {
      ValidateIssuerSigningKey = true,
      ValidIssuer = jwtOptions.Issuer,
      ValidAudience = jwtOptions.Audience,
      IssuerSigningKey = issuerSigningKey,
      NameClaimType = "Name",
      RoleClaimType = "Role",
      ClockSkew = TimeSpan.Zero,//default 300
    };
    builder.Services.AddSingleton(tokenValidationParameters);
    builder.Services.AddAuthentication(options =>
    {
      options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
      options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(o =>
    {
      o.TokenValidationParameters = tokenValidationParameters;
      o.Events = new JwtBearerEvents
      {
        OnAuthenticationFailed = c =>
        {
          return Task.CompletedTask;
        }
      };
    });
    builder.Services.AddAuthorization();
  }

  private static void AddMvc(WebApplicationBuilder builder)
  {
    builder.Services.AddRouting(options => options.ConstraintMap["slugify"] = typeof(SlugifyParameterTransformer));

    // MVC
    builder.Services.AddMvc(options =>
    {
      options.Conventions.Add(new GenericControllerRouteConvention());
      options.ModelMetadataDetailsProviders.Insert(0, new CustomDisplayMetadataProvider());
      // 小写 + 连字符格式
      options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer()));
    })
    .AddJsonOptions(options =>
    {
      options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
      if (builder.Environment.IsDevelopment())
      {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
        options.JsonSerializerOptions.WriteIndented = true;
      }
    })
    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
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
    builder.Services.AddSingleton<IStringLocalizer>(o => o.GetRequiredService<IStringLocalizer<Resource>>());
    builder.Services.AddPortableObjectLocalization(options => options.ResourcesPath = "Resources");
    builder.Services.Configure<RequestLocalizationOptions>(options =>
    {
      var supportedCultures = new List<CultureInfo>
      {
        new CultureInfo("zh"),
        new CultureInfo("en")
      };

      options.DefaultRequestCulture = new RequestCulture(supportedCultures.First());
      options.SupportedCultures = supportedCultures;
      options.SupportedUICultures = supportedCultures;
      //options.RequestCultureProviders.Clear();
      //options.RequestCultureProviders.Add(new RouteDataRequestCultureProvider());
    });
  }

  private static void AddSwagger(WebApplicationBuilder builder)
  {
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
      options.AddSecurityDefinition("bearerAuth", new OpenApiSecurityScheme
      {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
      });
      options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "bearerAuth" }
                },
                Array.Empty<string>()
            }
        });
    });
  }

  private static void AddCache(WebApplicationBuilder builder)
  {
    builder.Services.AddMemoryCache();
    var cacheConnectionName = builder.Configuration.GetConnectionString("cache");
    if (cacheConnectionName == "redis")
    {
      builder.Services.AddStackExchangeRedisCache(options =>
      {
        options.Configuration = builder.Configuration.GetConnectionString(cacheConnectionName);
        // options.InstanceName = "DefaultInstance";
      });
    }
    else
    {
      builder.Services.AddDistributedMemoryCache();
    }
  }

  private static void AddDbContext(WebApplicationBuilder builder)
  {
    builder.Services.AddScoped<DbContext, DefaultDbContext>();
    builder.Services.AddScoped<IDbSeed, DefaultDbSeed>();
    builder.Services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
    var dbConnectionName = builder.Configuration.GetConnectionString("database");
    var connectionString = builder.Configuration.GetConnectionString(dbConnectionName!);
    builder.Services.AddPooledDbContextFactory<DefaultDbContext>(
        options =>
        {
          if (dbConnectionName == "mysql")
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
