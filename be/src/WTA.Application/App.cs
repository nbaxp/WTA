namespace WTA.Application;

/// <summary>
/// 提供应用程序级别的 IServiceProvider 实例，主要用于解决扩展方法中无法使用依赖注入的问题
/// </summary>
public class App
{
    static App()
    {
        Current = new App();
    }

    private App()
    {
    }

    public static App Current { get; }
    public IServiceProvider? Services { get; private set; }

    public static void Init(IServiceProvider serviceProvider)
    {
        Current.Services = serviceProvider;
    }
}
