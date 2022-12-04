namespace WTA.Application.EventBus;

public class EntityCreatingEvent<T> : BaseEvent<T>
{
  public EntityCreatingEvent(T entity) : base(entity)
  {
  }
}
