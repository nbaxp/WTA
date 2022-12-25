namespace WTA.Application.Abstractions.Domain;

public abstract class TreeEntity<T> : BaseEntity where T : BaseEntity
{
  public string Name { get; set; }
  public string Number { get; set; }

  // public string Path { get; set; }
  public Guid? ParentId { get; set; }

  public T? Parent { get; set; }
  public List<T> Children { get; set; } = new List<T>();
}
