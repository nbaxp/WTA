using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using WTA.Application.Abstractions.EventBus;

namespace WTA.Infrastructure.EventBus;

public static class EventBusExtensions
{
    public static void AddEventBus(this IServiceCollection services, Func<Assembly, bool>? predicate = null)
    {
        services.AddEventBus<DefaultEventPublisher>(predicate);
    }

    public static void AddEventBus<T>(this IServiceCollection services, Func<Assembly, bool>? predicate) where T : class, IEventPublisher
    {
        services.AddTransient<IEventPublisher, T>();
        AppDomain.CurrentDomain.GetAssemblies().WhereIf(predicate != null, predicate).SelectMany(o => o.GetTypes())
          .Where(t => t.GetInterfaces().Any(o => o.IsGenericType && o.GetGenericTypeDefinition() == typeof(IEventHander<>)))
          .ToList()
          .ForEach(type =>
          {
              type.GetInterfaces()
              .Where(o => o.IsGenericType && o.GetGenericTypeDefinition() == typeof(IEventHander<>)).ToList()
              .ForEach(o => services.AddTransient(o, type));
          });
    }
}
