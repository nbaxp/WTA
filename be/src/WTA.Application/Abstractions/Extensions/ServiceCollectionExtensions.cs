using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WTA.Application.Abstractions.Components;

namespace WTA.Application.Abstractions.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// 根据 ImplementationAttribute 注册接口的默认实现
    /// </summary>
    public static void AddDefaultServices(this IServiceCollection services, Func<Assembly, bool>? predicate)
    {
        AppDomain.CurrentDomain.GetAssemblies(predicate).SelectMany(o => o.GetTypes())
          .Where(type => type.GetCustomAttributes(typeof(ServiceAttribute<>)).Any())
          .ForEach(type =>
          {
              var attribute = type.GetCustomAttribute(typeof(ServiceAttribute<>))!;
              if (attribute is IServiceAttribute implementation)
              {
                  var descriptor = new ServiceDescriptor(implementation.ServiceType, type, implementation.Lifetime);
                  services.Add(descriptor);
              }
          });
    }

    public static void AddDefaultOptions(this IServiceCollection services, IConfiguration configuration, Func<Assembly, bool>? predicate)
    {
        AppDomain.CurrentDomain.GetAssemblies(predicate).SelectMany(o => o.GetTypes())
          .Where(type => type.GetCustomAttributes<ConfigurationAttribute>().Any())
          .ForEach(type =>
          {
              var attribute = type.GetCustomAttribute<ConfigurationAttribute>()!;
              var configureMethod = typeof(OptionsConfigurationServiceCollectionExtensions)
                                      .GetMethod(nameof(OptionsConfigurationServiceCollectionExtensions.Configure),
                                          new[] { typeof(IServiceCollection), typeof(IConfiguration) });
              var configurationSection = configuration.GetSection(attribute.Section ?? type.Name.TrimEnd("Options"));
              configureMethod?.MakeGenericMethod(type).Invoke(null, new object[] { services, configurationSection });
          });
    }
}
