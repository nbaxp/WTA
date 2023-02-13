using System.Diagnostics;
using ExpressionTreeToString;
using OneOf;
using System.Linq.Expressions;
using ZSpitz.Util;

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

    public static string ExpressionToString(Expression expression)
    {
        return expression.ToString("Object notation", "C#");
    }

}
