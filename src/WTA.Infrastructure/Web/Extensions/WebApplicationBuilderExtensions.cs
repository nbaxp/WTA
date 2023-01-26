using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Localization.Routing;
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
using WTA.Application.Abstractions;
using WTA.Application.Abstractions.Data;
using WTA.Application.Abstractions.Include;
using WTA.Application.Abstractions.Url;
using WTA.Application.Authentication;
using WTA.Application.Domain.System;
using WTA.Application.Services;
using WTA.Application.Services.Permissions;
using WTA.Application.Services.Users;
using WTA.Infrastructure.Data;
using WTA.Infrastructure.EventBus;
using WTA.Infrastructure.Mapper;
using WTA.Infrastructure.Services;
using WTA.Infrastructure.Uri;
using WTA.Infrastructure.Web.Authentication;
using WTA.Infrastructure.Web.DataAnnotations;
using WTA.Infrastructure.Web.GenericControllers;
using WTA.Infrastructure.Web.Localization;
using WTA.Infrastructure.Web.Routing;
using WTA.Infrastructure.Web.Swagger;
using WTA.Resources;

namespace WTA.Infrastructure.Web.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static void BuildAndRun(this WebApplicationBuilder builder, string? serviceName = null)
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_HOSTINGSTARTUPASSEMBLIES", "SkyAPM.Agent.AspNetCore");
        Environment.SetEnvironmentVariable("SKYWALKING__SERVICENAME", serviceName ?? Assembly.GetEntryAssembly()!.GetName().Name);

        Serilog.Debugging.SelfLog.Enable(Console.Error);
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
            .CreateBootstrapLogger();
        try
        {
            Log.Information("Starting web application");

            if (builder.Configuration.GetValue("UseNacos", false))
            {
                builder.Host.UseNacosConfig("NacosConfig");
            }
            builder.Host.UseSerilog((hostingContext, services, configBuilder) =>
            {
                configBuilder
                .ReadFrom.Configuration(hostingContext.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
                .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture);
            }, writeToProviders: true);
            builder.Configure();
            builder.Services.AddSkyAPM();

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
            options.SerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
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
        AddCors(builder);
        AddDbContext(builder);
    }

    private static void AddServices(WebApplicationBuilder builder)
    {
        builder.Services.Configure<IdentityOptions>(builder.Configuration.GetSection(IdentityOptions.Position));
        builder.Services.AddHttpClient();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, SwaggerConfigureOptions>();
        builder.Services.AddEventBus();
        builder.Services.AddSingleton<ILinqInclude, DefaultLinqInclude>();
        builder.Services.AddSingleton<ILinqDynamic, DefaultLinqDynamic>();
        builder.Services.AddSingleton<IMapper, DefaultMapper>();
        builder.Services.AddSingleton<IUrlService, DefaultUrlService>();
        builder.Services.AddSingleton<ITokenService, TokenService>();
        builder.Services.AddTransient<IGuidGenerator, DefaultGuidGenerator>();
        builder.Services.AddTransient<IPasswordHasher, PasswordHasher>();
        builder.Services.AddTransient<IUserService, UserService>();
        builder.Services.AddTransient<IPermissionService, PermissionService>();
        builder.Services.AddTransient<ITestService<User>, TestService>();
        builder.Services.AddScoped<ITenantService, TenantService>();

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
        builder.Services.Configure<RazorViewEngineOptions>(options =>
        {
            options.ViewLocationFormats.Clear();
            options.ViewLocationFormats.Add("/Views/{1}/{0}" + RazorViewEngine.ViewExtension);
            options.ViewLocationFormats.Add("/Views/Shared/{0}" + RazorViewEngine.ViewExtension);
            options.ViewLocationFormats.Add("/Views/Shared/Default" + RazorViewEngine.ViewExtension);

            options.AreaViewLocationFormats.Add("/Areas/{2}/Views/{1}/{0}" + RazorViewEngine.ViewExtension);
            options.AreaViewLocationFormats.Add("/Areas/{2}/Views/Shared/{0}" + RazorViewEngine.ViewExtension);
            options.AreaViewLocationFormats.Add("/Views/Shared/{0}" + RazorViewEngine.ViewExtension);
            options.AreaViewLocationFormats.Add("/Views/Shared/Default" + RazorViewEngine.ViewExtension);
        });
        builder.Services.AddRouting(options => options.ConstraintMap["slugify"] = typeof(SlugifyParameterTransformer));

        // MVC
        builder.Services.AddMvc(options =>
        {
            options.ModelMetadataDetailsProviders.Insert(0, new DefaultDisplayMetadataProvider());
            options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer()));
            options.Conventions.Add(new GenericControllerRouteConvention());
            //options.ModelBindingMessageProvider
        })
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
            if (builder.Environment.IsDevelopment())
            {
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
        .ConfigureApiBehaviorOptions(options =>
        {
            options.SuppressModelStateInvalidFilter = true;
            //options.InvalidModelStateResponseFactory = context =>
            //{
            //    context.ActionDescriptor.Parameters[0].ParameterType.GetMetadataForType(context.HttpContext.RequestServices);
            //    if (!context.ModelState.IsValid)
            //    {
            //        var errors = context.ModelState.ToErrors();
            //    }
            //    return new BadRequestObjectResult(context.ModelState);
            //};
        })
        .ConfigureApplicationPartManager(o => o.FeatureProviders.Add(new GenericControllerFeatureProvider()))
        .AddControllersAsServices();// must after ConfigureApplicationPartManager
    }

    private static void AddLocalization(WebApplicationBuilder builder)
    {
        builder.Services.AddLocalization();
        builder.Services.AddSingleton<IStringLocalizerFactory, JsonStringLocalizerFactory>();
        builder.Services.AddSingleton<IStringLocalizer>(o => o.GetRequiredService<IStringLocalizer<Resource>>());
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
            options.RequestCultureProviders.Insert(0, new RouteDataRequestCultureProvider());
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

    private static void AddCors(WebApplicationBuilder builder)
    {
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("Default", builder =>
            {
                builder.SetIsOriginAllowed(isOriginAllowed => true)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
            });
        });
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
