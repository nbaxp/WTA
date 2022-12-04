namespace WTA.Application.EventBus;

public class BaseEvent<T>
{
  public BaseEvent(T data)
  {
    this.Id = Guid.NewGuid();
    this.CreationDate = DateTime.UtcNow;
    Data = data;
  }

  public Guid Id { get; }
  public DateTime CreationDate { get; }
  public T Data { get; }
}
