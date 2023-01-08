using Microsoft.AspNetCore.Mvc;
using WTA.Application.Abstractions.Data;
using WTA.Application.Domain.System;
using WTA.Infrastructure.Web.Extensions;
using WTA.Web.Models;

namespace WTA.Web.Controllers;

[Route("[controller]/[action]")]
[ApiController]
public class TestApiController : Controller
{
    private readonly IRepository<User> _userRepository;

    public TestApiController(IRepository<User> userRepository)
    {
        this._userRepository = userRepository;
    }

    [HttpGet]
    public IActionResult Index([FromQuery] TestModel model)
    {
        if (ModelState.IsValid)
        {
        }
        return Json(new
        {
            Model = model,
            Errors = ViewData.ModelState.ToErrors(),
            //Schema1 = this.ViewData.ModelMetadata.GetSchema(this.HttpContext.RequestServices),
            Schema = model.GetType().GetMetadataForType(this.HttpContext.RequestServices)
        });
    }

    [HttpGet]
    public IActionResult Query([FromQuery] TestModel model)
    {
        if (ModelState.IsValid)
        {
        }
        var list = this._userRepository.AsNoTracking()
            //.Where(model)
            .Where("UserName.Contains(@0) and (UserName.StartsWith(@1) or UserName.EndsWith(@2))", "mi", "ad", "in")
            .ToList();
        return Json(new
        {
            Test = list,
            Model = model,
            Errors = ViewData.ModelState.ToErrors(),
            Schema = model.GetType().GetMetadataForType(this.HttpContext.RequestServices)
        });
    }
}
