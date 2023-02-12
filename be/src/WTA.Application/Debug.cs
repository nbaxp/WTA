using System.Diagnostics;

namespace WTA.Application;

public class Debug
{
    public static void Watch(Action action)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        action.Invoke();
        stopwatch.Stop();
        Console.WriteLine($"{action.ToString()}:{stopwatch.ElapsedMilliseconds / 1000.0:f3}");
    }
}
