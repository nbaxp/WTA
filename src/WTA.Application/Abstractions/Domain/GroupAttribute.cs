namespace WTA.Application.Abstractions.Domain;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class GroupAttribute : Attribute
{
    public GroupAttribute(string? group = null, string? area = null)
    {
        Group = group;
        Area = area;
    }

    public string? Group { get; set; }
    public string? Area { get; set; }
}
