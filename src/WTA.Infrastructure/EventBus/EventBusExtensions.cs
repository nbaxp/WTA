using Microsoft.Extensions.DependencyInjection;
using WTA.Application.Abstractions.EventBus;

namespace WTA.Infrastructure.EventBus;

public static class EventBusExtensions
{
    public static void AddEventBus(this IServiceCollection services)
    {
        services.AddTransient<IEventPublisher, EventPublisher>();
        AppDomain.CurrentDomain.GetAssemblies().SelectMany(o => o.GetTypes())
          .Where(t => t.GetInterfaces().Any(o => o.IsGenericType && o.GetGenericTypeDefinition() == typeof(IEventHander<>)))
          .ToList()
          .ForEach(t =>
          {
              t.GetInterfaces().Where(o => o.IsGenericType && o.GetGenericTypeDefinition() == typeof(IEventHander<>)).ToList().ForEach(o =>
          {
              services.AddTransient(o, t);
          });
          });
    }
}
