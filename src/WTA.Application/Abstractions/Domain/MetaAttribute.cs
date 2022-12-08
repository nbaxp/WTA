namespace WTA.Application.Abstractions.Domain;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class MetaAttribute : Attribute
{
  public MetaAttribute(string? group = null)
  {
    Group = group;
  }

  public string? Group { get; set; }
}
