namespace WTA.Application.Abstractions.EventBus;

public class EntityDeletingEvent<T> : BaseEvent<T>
{
  public EntityDeletingEvent(T entity) : base(entity)
  {
  }
}
