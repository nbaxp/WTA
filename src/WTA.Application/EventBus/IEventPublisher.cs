namespace WTA.Application.EventBus;

public interface IEventPublisher
{
  Task Publish<T>(T data);
}
