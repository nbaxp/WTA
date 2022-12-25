using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WTA.Application.Abstractions.EventBus;
using WTA.Application.Domain.System;
using WTA.Application.Services;
using WTA.Web.Events;

namespace WTA.Web.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class TestWebController : Controller
{
  private readonly IEventPublisher _eventPublisher;
  private readonly ITestService<User> _testService;

  public TestWebController(IEventPublisher eventPublisher, ITestService<User> testService)
  {
    this._eventPublisher = eventPublisher;
    this._testService = testService;
  }

  [HttpGet]
  public async Task<IActionResult> Index()
  {
    var result = this._testService.Test();
    await this._eventPublisher.Publish(new TestEvent(new User(), "test"));
    return Content("Test Web Controller");
  }

  [HttpGet]
  [Authorize]
  public IActionResult Test1()
  {
    return Content("");
  }

  [HttpGet]
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
