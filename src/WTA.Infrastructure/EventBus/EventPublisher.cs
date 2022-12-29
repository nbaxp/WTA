using Microsoft.Extensions.DependencyInjection;
using WTA.Application.Abstractions.EventBus;
using WTA.Application.Abstractions.Extensions;

namespace WTA.Infrastructure.EventBus;

public class EventPublisher : IEventPublisher
{
    private readonly IServiceProvider _applicationServices;

    public EventPublisher(IServiceProvider applicationServices)
    {
        this._applicationServices = applicationServices;
    }

    public async Task Publish<T>(T data)
    {
        using var scope = _applicationServices.CreateScope();
        var subscribers = scope.ServiceProvider.GetServices<IEventHander<T>>().ToList();
        foreach (var item in subscribers)
        {
            try
            {
                await item.Handle(data).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ex.PrintStack();
                throw;
            }
        }
    }
}
