namespace WTA.Application.Core;

public interface ITreeNode<T> where T : BaseEntity
{
  string Name { get; set; }
  string Number { get; set; }
  string Path { get; set; }
  Guid? ParentId { get; set; }
  public T? Parent { get; set; }
  List<T> Children { get; set; }
}
