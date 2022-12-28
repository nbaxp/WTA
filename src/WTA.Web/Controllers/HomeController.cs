using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WTA.Application.Abstractions.EventBus;
using WTA.Application.Domain.System;
using WTA.Application.Services;
using WTA.Web.Events;

namespace WTA.Web.Controllers;

public class HomeController : Controller
{
  private readonly IEventPublisher _eventPublisher;
  private readonly ITestService<User> _testService;

  public HomeController(IEventPublisher eventPublisher, ITestService<User> testService)
  {
    this._eventPublisher = eventPublisher;
    this._testService = testService;
  }

  public async Task<IActionResult> Test()
  {
    var result = this._testService.Test();
    await this._eventPublisher.Publish(new TestEvent(new User(), "test"));
    return Content("Test Web Controller");
  }

  [Authorize]
  public IActionResult Test1()
  {
    return Content("");
  }

  [Authorize(Roles = "System")]
  public IActionResult Test2()
  {
    return Content("");
  }
}

public class EventTestHandler : IEventHander<TestEvent>
{
  public Task Handle(TestEvent data)
  {
    return Task.CompletedTask;
  }
}
