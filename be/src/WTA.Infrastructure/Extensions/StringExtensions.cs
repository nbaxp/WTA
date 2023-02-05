namespace WTA.Infrastructure.Extensions;

public static class StringExtensions
{
    public static string TrimEnd(this string input, string value)
    {
        return input.EndsWith(value) ? input.Substring(0, input.Length - value.Length) : input;
    }
}
