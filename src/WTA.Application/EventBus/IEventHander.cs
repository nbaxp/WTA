namespace WTA.Application.EventBus;

public interface IEventHander<T>
{
  Task Handle(T data);
}
