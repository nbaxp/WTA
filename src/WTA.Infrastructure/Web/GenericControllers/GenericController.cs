using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WTA.Application.Abstractions.Data;
using WTA.Application.Abstractions.Extensions;
using WTA.Infrastructure.Web.Extensions;

namespace WTA.Infrastructure.Web.GenericControllers;

[GenericControllerNameConvention]
public class GenericController<TEntity, TDisplayModel, TEditModel> : Controller
  where TEntity : class
  where TDisplayModel : class
  where TEditModel : class
{
  private readonly IRepository<TEntity> _repository;

  public GenericController(IRepository<TEntity> repository)
  {
    this._repository = repository;
  }

  [HttpGet]
  public async Task<IActionResult> Index([FromQuery] PaginationViewModel<TEntity> model)
  {
    var meta = HttpContext.RequestServices.GetRequiredService<ModelMetadataProvider>().GetMetadataForType(typeof(TEntity));
    try
    {
      if (ModelState.IsValid)
      {
        var query = this._repository.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(model.Query))
        {
          query = query.Where(model.Query);
        }
        model.TotalCount = await query.CountAsync();
        if (!string.IsNullOrWhiteSpace(model.OrderBy))
        {
          query = query.OrderBy(model.OrderBy);
        }
        model.Items = await query.Skip(model.PageSize * (model.PageIndex - 1)).Take(model.PageSize).ToListAsync();
        return Json(new
        {
          model,
          schema = ViewData.ModelMetadata.GetSchema(HttpContext.RequestServices, true)
        });
      }
      return BadRequest();
    }
    catch (Exception ex)
    {
      return Problem(ex.Message);
    }
  }

  //[HttpGet("{id}")]
  //public virtual async Task<IActionResult> Details(Guid? id)
  //{
  //  if (id == null)
  //  {
  //    return BadRequest();
  //  }
  //  var entity = await this._repository.FindAsync(id.Value);
  //  if (entity == null)
  //  {
  //    return NotFound();
  //  }
  //  var model = entity?.To<TEditModel>();
  //  return Json(new
  //  {
  //    model,
  //    schema = ViewData.ModelMetadata.GetSchema(HttpContext.RequestServices, true)
  //  });
  //}

  //[HttpGet]
  //public virtual IActionResult Create()
  //{
  //  var model = Activator.CreateInstance(typeof(TEditModel));
  //  return this.Result(model);
  //}

  [HttpPost]
  [ValidateAntiForgeryToken]
  public virtual async Task<IActionResult> Create([FromBody] TEditModel model)
  {
    if (ModelState.IsValid)
    {
      var entity = model.To<TEntity>();
      await this._repository.AddAsync(entity);
      await _repository.SaveChangesAsync();
      return Ok();
    }
    return BadRequest();
  }
}
