namespace WTA.Application;

/// <summary>
/// 提供应用程序级别的 IServiceProvider 实例，主要用于解决扩展方法中无法使用依赖注入的问题
/// </summary>
public class AppContext
{
  public IServiceProvider? Services { get; private set; }

  public static void Configure(IServiceProvider serviceProvider)
  {
    Current.Services = serviceProvider;
  }

  public static AppContext Current = new AppContext();
}
