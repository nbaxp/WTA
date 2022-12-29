namespace WTA.Application.Abstractions.EventBus;

public class EntityUpdatingEvent<T> : BaseEvent<T>
{
    public EntityUpdatingEvent(T entity) : base(entity)
    {
    }
}
