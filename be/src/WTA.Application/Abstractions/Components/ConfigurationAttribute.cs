namespace WTA.Application.Abstractions.Components;

[AttributeUsage(AttributeTargets.Class)]
public class ConfigurationAttribute : Attribute
{
    public ConfigurationAttribute(string? section = null)
    {
        Section = section;
    }

    public string? Section { get; }
}
