namespace WTA.Application.Abstractions.Extensions;

public static class ExceptionExtensions
{
    public static void PrintStack(this Exception exception)
    {
        Console.WriteLine(exception.Message ?? exception.InnerException?.Message);
        Console.WriteLine(exception.StackTrace ?? exception.InnerException?.StackTrace);
    }
}
