namespace WTA.Application.Abstractions.EventBus;

public class EntityCreatingEvent<T> : BaseEvent<T>
{
    public EntityCreatingEvent(T entity) : base(entity)
    {
    }
}
