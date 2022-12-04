using System.Diagnostics;

namespace WTA.Application.Extensions;

public static class ExceptionExtensions
{
  public static void PrintStack(this Exception exception)
  {
    Debug.WriteLine(exception.Message ?? exception.InnerException.Message);
    Debug.WriteLine(exception.StackTrace ?? exception.InnerException.StackTrace);
  }
}
