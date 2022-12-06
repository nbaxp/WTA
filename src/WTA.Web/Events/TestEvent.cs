using WTA.Application.Abstractions.EventBus;
using WTA.Application.Domain.Users;

namespace WTA.Web.Events
{
  public class TestEvent : EntityCreatingEvent<User>
  {
    public TestEvent(User entity, string test) : base(entity)
    {
      this.Test = test;
    }

    public string? Test { get; }
  }
}
