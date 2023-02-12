using Microsoft.Extensions.DependencyInjection;

namespace WTA.Application.Abstractions.Extensions;

public static class ApplicationContextExtensions
{
    /// <summary>
    /// 使用 IGuidGenerator 的具体实现生成自增的 GUID
    /// </summary>
    public static Guid NewGuid(this App appContext)
    {
        using var scope = App.Current.Services!.CreateScope();
        return scope.ServiceProvider.GetRequiredService<IGuidGenerator>().Create();
    }
}
