using System.ComponentModel.DataAnnotations;

namespace WTA.Application.Abstractions.Extensions;

public static class EnumExtensions
{
    public static int GetValue(this Enum value)
    {
        return (int)Enum.ToObject(value.GetType(), value);
    }

    public static string GetDisplayName(this Enum value)
    {
        var type = value.GetType();
        var name = Enum.GetName(type, value)!;
        var fieldInfo = type.GetField(name);
        var attribute = Attribute.GetCustomAttribute(fieldInfo!, typeof(DisplayAttribute)) as DisplayAttribute;
        return attribute?.Name ?? name;
    }
}
