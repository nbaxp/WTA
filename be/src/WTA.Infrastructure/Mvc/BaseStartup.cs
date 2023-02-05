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
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Localization.Routing;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Debugging;
using Swashbuckle.AspNetCore.SwaggerGen;
using WTA.Application.Abstractions.Data;
using WTA.Application.Authentication;
using WTA.Infrastructure.Authentication;
using WTA.Infrastructure.Data;
using WTA.Infrastructure.DataAnnotations;
using WTA.Infrastructure.Extensions;
using WTA.Infrastructure.GenericControllers;
using WTA.Infrastructure.Localization;
using WTA.Infrastructure.Routing;
using WTA.Infrastructure.Swagger;
using WTA.Resources;

namespace WTA.Infrastructure.Mvc;

public class BaseStartup
{
    private readonly string _name;

    public BaseStartup()
    {
        _name = Assembly.GetEntryAssembly()?.GetName().Name!;
    }

    public virtual void Configure(WebApplication app)
    {
        UseStaticFiles(app);
        UseRouting(app);
        UseLocalization(app);
        UseSwagger(app);
        UseAuthorization(app);
        UseDatabase(app);
    }

    public virtual void ConfigureServices(WebApplicationBuilder builder)
    {
        AddHttp(builder);
        AddLocalization(builder);
        AddMvc(builder);
        AddSwagger(builder);
        AddAuthentication(builder);
        AddCache(builder);
        AddDbContext(builder);
    }

    public void Start(string[] args)
    {
        var aspNetCoreEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        if (aspNetCoreEnvironment == "Development")
        {
            SelfLog.Enable(Console.WriteLine);
        }
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
            .CreateBootstrapLogger();
        try
        {
            Log.Information($"{_name} start");
            var builder = WebApplication.CreateBuilder(args);
            builder.Host.UseSerilog((hostingContext, services, configBuilder) =>
            {
                configBuilder
                .ReadFrom.Configuration(hostingContext.Configuration)
                .Enrich.FromLogContext()
                .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture);
            }, writeToProviders: true);
            this.ConfigureServices(builder);
            var app = builder.Build();
            app.UseSerilogRequestLogging();
            this.Configure(app);
            app.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, ex.Message);
        }
        finally
        {
            Log.Information($"{_name} stop");
            Log.CloseAndFlush();
        }
    }

    #region add services

    protected virtual void AddAuthentication(WebApplicationBuilder builder)
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
                OnMessageReceived = context =>
                {
                    if (!context.Request.IsJsonRequest() && context.Request.Cookies.TryGetValue("access_key", out var token))
                    {
                        context.Token = token;
                    }
                    return Task.CompletedTask;
                },
                OnChallenge = context =>
                {
                    if (!context.Request.IsJsonRequest())
                    {
                        var linkGenerator = context.HttpContext.RequestServices.GetRequiredService<LinkGenerator>();
                        var url = linkGenerator.GetPathByAction("Login", "Account", new { returnUrl = context.Request.GetDisplayUrl() }, pathBase: context.HttpContext.Request.PathBase);
                        context.Response.Redirect(url!);
                        context.HandleResponse();
                    }
                    return Task.CompletedTask;
                },
            };
        });
        builder.Services.AddAuthorization();
    }

    protected virtual void AddCache(WebApplicationBuilder builder)
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

    protected virtual void AddDbContext(WebApplicationBuilder builder)
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

    protected virtual void AddHttp(WebApplicationBuilder builder)
    {
        builder.Services.AddHttpClient();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddCors(options =>
        {
            options.AddPolicy(_name!, builder =>
            {
                builder.SetIsOriginAllowed(isOriginAllowed => true)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
            });
        });
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
    }

    protected virtual void AddLocalization(WebApplicationBuilder builder)
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

    protected virtual void AddMvc(WebApplicationBuilder builder)
    {
        builder.Services.Configure<RazorViewEngineOptions>(options =>
        {
            options.ViewLocationFormats.Add("/Views/{1}/{0}" + RazorViewEngine.ViewExtension);
            options.ViewLocationFormats.Add("/Views/Shared/{0}" + RazorViewEngine.ViewExtension);
            options.ViewLocationFormats.Add("/Views/Shared/Default" + RazorViewEngine.ViewExtension);// add for default

            options.AreaViewLocationFormats.Add("/Areas/{2}/Views/{1}/{0}" + RazorViewEngine.ViewExtension);
            options.AreaViewLocationFormats.Add("/Areas/{2}/Views/Shared/{0}" + RazorViewEngine.ViewExtension);
            options.AreaViewLocationFormats.Add("/Areas/{2}/Views/Shared/Default" + RazorViewEngine.ViewExtension);// add for default
            options.AreaViewLocationFormats.Add("/Views/Shared/{0}" + RazorViewEngine.ViewExtension);
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

    protected virtual void AddSwagger(WebApplicationBuilder builder)
    {
        builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, SwaggerConfigureOptions>();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.DocumentFilter<SwaggerFilter>();
            options.OperationFilter<SwaggerFilter>();
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

    #endregion add services

    #region configure

    protected virtual void UseStaticFiles(WebApplication app)
    {
        // app 下载配置
        var provider = new FileExtensionContentTypeProvider();
        provider.Mappings.Add(".apk", "application/vnd.android.package-archive");
        provider.Mappings.Add(".plist", "text/xml");
        provider.Mappings.Add(".ipa", "application/iphone");
        app.UseStaticFiles(new StaticFileOptions
        {
            ContentTypeProvider = provider,
            ServeUnknownFileTypes = true,
            DefaultContentType = "application/octet-stream"
        });
    }

    protected virtual void UseRouting(WebApplication app)
    {
        var requestLocalizationOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value;
        var defaults = new { culture = requestLocalizationOptions.DefaultRequestCulture.Culture.Name };
        app.UseRouting();
        app.MapControllerRoute(name: "area", pattern: "{area:exists:slugify}/{controller:slugify=Home}/{action:slugify=Index}/{id?}", defaults: defaults);
        app.MapControllerRoute(name: "default", pattern: "{controller:slugify=Home}/{action:slugify=Index}/{id?}", defaults: defaults);
    }

    protected virtual void UseLocalization(WebApplication app)
    {
        var localizationOptions = app.Services.GetService<IOptions<RequestLocalizationOptions>>()!.Value;
        app.UseRequestLocalization(localizationOptions);
        Thread.CurrentThread.CurrentCulture = localizationOptions.DefaultRequestCulture.Culture;
        Thread.CurrentThread.CurrentUICulture = localizationOptions.DefaultRequestCulture.UICulture;
    }

    protected virtual void UseSwagger(WebApplication app)
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

    protected virtual void UseAuthorization(WebApplication app)
    {
        app.UseCors(_name);
        app.UseAuthentication();
        app.UseAuthorization();
    }

    protected virtual void UseDatabase(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        using var db = scope.ServiceProvider.GetRequiredService<DbContext>();
        if (db.Database.EnsureCreated())
        {
            scope.ServiceProvider.GetRequiredService<IDbSeed>().Initialize();
        }
    }

    #endregion configure
}
