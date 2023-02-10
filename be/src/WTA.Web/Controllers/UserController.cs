using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using WTA.Application.Abstractions.Data;
using WTA.Application.Domain.System;
using WTA.Infrastructure.Extensions;
using WTA.Infrastructure.GenericControllers;

namespace WTA.Web.Controllers;

[Route("[controller]/[action]")]
[ApiController]
public class UserController : GenericController<User, User, User, PaginationViewModel<User>>
{
    public UserController(IRepository<User> repository) : base(repository)
    {
        var filter = new TestModel { UserName = "admin" }.CreateExpression<User>();
        // var users = repository.Query().WhereIf(filter != null, filter).ToList();
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
