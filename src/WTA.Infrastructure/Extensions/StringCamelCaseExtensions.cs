namespace WTA.Infrastructure.Extensions;

public static class StringCamelCaseExtensions
{
    public static string ToLowerCamelCase(this string input)
    {
        if (string.IsNullOrEmpty(input) || !char.IsUpper(input[0]))
        {
            return input;
        }
        var chars = input.ToCharArray();
        FixCasing(chars);
        return new string(chars);
    }

    private static void FixCasing(Span<char> chars)
    {
        for (var i = 0; i < chars.Length; i++)
        {
            if (i == 1 && !char.IsUpper(chars[i]))
            {
                break;
            }

            var hasNext = i + 1 < chars.Length;

            // Stop when next char is already lowercase.
            if (i > 0 && hasNext && !char.IsUpper(chars[i + 1]))
            {
                // If the next char is a space, lowercase current char before exiting.
                if (chars[i + 1] == ' ')
                {
                    chars[i] = char.ToLowerInvariant(chars[i]);
                }

                break;
            }

            chars[i] = char.ToLowerInvariant(chars[i]);
        }
    }
}
