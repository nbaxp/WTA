namespace WTA.Application.Abstractions.Domain;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class ControllerAttribute : Attribute
{
  public ControllerAttribute(string area = "", string name = "")
  {
    Area = area;
    Name = name;
  }

  public string Area { get; set; }
  public string Name { get; set; }
}
