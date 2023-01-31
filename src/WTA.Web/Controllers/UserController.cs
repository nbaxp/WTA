using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using WTA.Application.Abstractions.Data;
using WTA.Application.Domain.System;
using WTA.Application.Services.Users;
using WTA.Infrastructure.Web.Extensions;
using WTA.Infrastructure.Web.GenericControllers;

namespace WTA.Web.Controllers;

[Route("[controller]/[action]")]
[ApiController]
public class UserController : GenericController<User, User, User, PaginationViewModel<User>>
{
    public UserController(IRepository<User> repository) : base(repository)
    {
        var filter = new TestModel { UserName = "admin" }.CreateExpression<User>();
        var users = repository.Query().Where(filter).ToList();
    }

    public override Task<IActionResult> Index([FromQuery] PaginationViewModel<User> model)
    {
        return base.Index(model);
    }
}

public class TestModel
{
    [Key]
    public string? UserName { get; set; }

    [Key]
    public bool TwoFactorEnabled { get; set; }
}
