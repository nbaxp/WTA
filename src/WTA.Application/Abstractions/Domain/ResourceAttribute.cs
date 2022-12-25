namespace WTA.Application.Abstractions.Domain;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class ResourceAttribute : Attribute
{
  public ResourceAttribute(string? group = null)
  {
    Group = group;
  }

  public string? Group { get; set; }
}
