using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WTA.Application.Data;
using WTA.Application.Domain.Users;
using WTA.Application.Interfaces;
using WTA.Application.Services;
using WTA.Infrastructure.Data;
using WTA.Infrastructure.EventBus;
using WTA.Infrastructure.Mapper;
using WTA.Infrastructure.Services;
using WTA.Infrastructure.Web.Routing;

namespace WTA.Infrastructure.Web.Extensions;

public static class WebApplicationBuilderExtensions
{
  public static void Config(this WebApplicationBuilder builder)
  {
    AddServices(builder);
    AddDbContext(builder);
    AddMvc(builder);
    AddSwagger(builder);
  }

  private static void AddServices(WebApplicationBuilder builder)
  {
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddEventBus();
    builder.Services.AddSingleton<ILinqInclude, DefaultLinqInclude>();
    builder.Services.AddSingleton<ILinqDynamic, DefaultLinqDynamic>();
    builder.Services.AddSingleton<IMapper, DefaultMapper>();
    builder.Services.AddTransient<IGuidGenerator, DefaultGuidGenerator>();
    builder.Services.AddScoped<ITenantService, TenantService>();
    builder.Services.AddTransient<IPasswordHasher, PasswordHasher>();
    builder.Services.AddTransient<ITestService<User>, TestService>();
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

  private static void AddMvc(WebApplicationBuilder builder)
  {
    builder.Services.AddRouting(options => options.ConstraintMap["slugify"] = typeof(SlugifyParameterTransformer));
    // MVC
    builder.Services.AddMvc(options =>
    {
      //options.ModelMetadataDetailsProviders.Insert(0, new CustomIDisplayMetadataProvider());
      // 小写 + 连字符格式
      options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer()));
    });
  }

  private static void AddSwagger(WebApplicationBuilder builder)
  {
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
  }
}
