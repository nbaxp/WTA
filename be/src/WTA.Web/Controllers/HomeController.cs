using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WTA.Application.Abstractions.Data;
using WTA.Application.Abstractions.EventBus;
using WTA.Application.Domain.System;
using WTA.Application.Services;
using WTA.Web.Events;

namespace WTA.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IEventPublisher _eventPublisher;

    public HomeController(ILogger<HomeController> logger,
        IEventPublisher eventPublisher,
        IRepository<User> repository,
        IApplicationService<Role> service1,
        IApplicationService<User> service2)
    {
        this._logger = logger;
        this._eventPublisher = eventPublisher;
        var users = repository.Query().AsNoTracking().Select(o => new { o.UserName, o.UserRoles }).ToList();
    }

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> Test()
    {
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
