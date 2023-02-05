using Microsoft.Extensions.DependencyInjection;

namespace WTA.Application.Abstractions.Components;

public interface IServiceAttribute
{
    ServiceLifetime Lifetime { get; }
    Type ServiceType { get; }
}
