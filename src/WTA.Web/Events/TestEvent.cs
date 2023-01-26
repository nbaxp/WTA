using WTA.Application.Abstractions.EventBus;
using WTA.Application.Domain.System;

namespace WTA.Web.Events;

public class TestEvent : EntityCreatedEvent<User>
{
    public TestEvent(User entity, string test) : base(entity)
    {
        this.Test = test;
    }

    public string? Test { get; }
}
