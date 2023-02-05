namespace WTA.Application.Abstractions.Extensions;

public static class StringNormalizeExtensions
{
    /// <summary>
    /// 字符串的大写格式，避免只有大小写不同的字符串
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string Normalized(this string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }
        return value.ToUpperInvariant();
    }
}
