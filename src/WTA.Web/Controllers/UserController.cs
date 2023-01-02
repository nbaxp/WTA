using Microsoft.AspNetCore.Mvc;
using WTA.Application.Abstractions.Data;
using WTA.Application.Domain.System;
using WTA.Infrastructure.Web.GenericControllers;

namespace WTA.Web.Controllers;

[Route("[controller]/[action]")]
[ApiController]
public class UserController : GenericController<User, User, User>
{
    public UserController(IRepository<User> repository) : base(repository)
    {
    }

    public override Task<IActionResult> Index([FromQuery] PaginationViewModel<User> model)
    {
        return base.Index(model);
    }
}
