using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WTA.Application.Abstractions.EventBus;
using WTA.Application.Domain.System;
using WTA.Application.Services;
using WTA.Web.Events;

namespace WTA.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IEventPublisher _eventPublisher;
    private readonly ITestService<User> _testService;

    public HomeController(ILogger<HomeController> logger, IEventPublisher eventPublisher, ITestService<User> testService)
    {
        this._logger = logger;
        this._eventPublisher = eventPublisher;
        this._testService = testService;
    }

    public async Task<IActionResult> Test()
    {
        _ = this._testService.Test();
        await _eventPublisher.Publish(new TestEvent(new User(), "test")).ConfigureAwait(false);
        return Content("Test Web Controller");
    }

    public IActionResult TestSkyWalking()
    {
        var message = $"test sky walking:{DateTime.Now.ToLongDateString()} {DateTime.Now.ToLongTimeString()}";
        this._logger.LogInformation(message);
        return Content(message);
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
