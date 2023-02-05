using System.Reflection;

namespace WTA.Application.Abstractions.Extensions;

public static class AppDomainExtensions
{
    /// <summary>
    /// 根据条件获取 AppDomain 下的程序集
    /// </summary>
    public static IEnumerable<Assembly> GetAssemblies(this AppDomain appDomain, Func<Assembly, bool>? predicate = null)
    {
        return appDomain.GetAssemblies().WhereIf(predicate != null, predicate!.Invoke);
    }
}
