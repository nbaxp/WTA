using Microsoft.AspNetCore.Mvc;
using WTA.Application.Domain.Users;
using WTA.Application.EventBus;
using WTA.Application.Services;
using WTA.Web.Events;

namespace WTA.Web.Controllers
{
  public class TestWebController : Controller
  {
    private readonly IEventPublisher _eventPublisher;
    private readonly ITestService<User> _testService;

    public TestWebController(IEventPublisher eventPublisher, ITestService<User> testService)
    {
      this._eventPublisher = eventPublisher;
      this._testService = testService;
    }

    public async Task<IActionResult> Index()
    {
      var result = this._testService.Test();
      await this._eventPublisher.Publish(new TestEvent(new User(), "test"));
      return Content("Test Web Controller");
    }
  }

  public class EventTestHandler : IEventHander<TestEvent>
  {
    public Task Handle(TestEvent data)
    {
      return Task.CompletedTask;
    }
  }
}
