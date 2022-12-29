using Microsoft.Extensions.DependencyInjection;

namespace WTA.Application.Abstractions.Extensions;

public static class ObjectMapperExtensions
{
    public static T To<T>(this object from)
    {
        using var scope = AppContext.Current.Services!.CreateScope();
        return scope.ServiceProvider.GetRequiredService<IMapper>().To<T>(from);
    }

    public static T From<T>(this T target, object from)
    {
        target.From(from);
        return target;
    }
}
