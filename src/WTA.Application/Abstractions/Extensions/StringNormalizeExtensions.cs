namespace WTA.Application.Abstractions.Extensions;

public static class StringNormalizeExtensions
{
    public static string Normalized(this string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }
        return value.ToUpperInvariant();
    }
}
