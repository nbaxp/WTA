namespace WTA.Application.Abstractions.EventBus;

public class BaseEvent<T>
{
  public BaseEvent(T data)
  {
    Id = Guid.NewGuid();
    CreationDate = DateTime.UtcNow;
    Data = data;
  }

  public Guid Id { get; }
  public DateTime CreationDate { get; }
  public T Data { get; }
}
