using System.Dynamic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WTA.Application.Abstractions.Data;
using WTA.Application.Abstractions.Extensions;
using WTA.Infrastructure.Extensions;

namespace WTA.Infrastructure.Controllers;

/// <summary>
/// 泛型控制器
/// </summary>
/// <typeparam name="TEntity">实体</typeparam>
/// <typeparam name="TModel">新建和编辑模型</typeparam>
/// <typeparam name="TListModel">列表项模型</typeparam>
/// <typeparam name="TSearchModel">查询模型</typeparam>
[GenericControllerNameConvention]
public class GenericController<TEntity, TModel, TListModel, TSearchModel> : Controller
  where TEntity : class
  where TModel : class
  where TListModel : class
  where TSearchModel : PaginationViewModel<TListModel>
{
    private readonly IRepository<TEntity> _repository;

    public GenericController(IRepository<TEntity> repository)
    {
        _repository = repository;
    }

    #region List

    [HttpGet]
    public virtual async Task<IActionResult> Index([FromQuery] TSearchModel model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var query = _repository.AsNoTracking();
                if (!string.IsNullOrWhiteSpace(model.Query))
                {
                    query = query.Query(model.Query);
                }
                model.TotalCount = await query.CountAsync().ConfigureAwait(false);
                if (!string.IsNullOrWhiteSpace(model.OrderBy))
                {
                    query = query.OrderBy(model.OrderBy);
                }
                model.Items = (await query.Skip(model.PageSize * (model.PageIndex - 1))
                    .Take(model.PageSize)
                    .ToListAsync()
                    .ConfigureAwait(false))
                    .To<List<TListModel>>();
                return Json(new
                {
                    model,
                    schema = model.GetType().GetMetadataForType(HttpContext.RequestServices, true)
                });
            }
            return BadRequest();
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    [HttpGet]
    public virtual IActionResult List([FromQuery] object model)
    {
        var query = _repository.AsNoTracking();
        query = query.Query(model);
        dynamic result = new ExpandoObject();
        result.Total = query.Count();
        var pageSize = model.GetPageSize();
        var pageIndex = model.GetPageIndex();
        result.List = query.Skip(pageSize * (pageIndex - 1))
            .Take(pageSize);
        return Json(result);
    }

    #endregion List

    #region Create/Edit/Delete

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

    [HttpGet]
    public virtual IActionResult Create()
    {
        var model = Activator.CreateInstance(typeof(TModel));
        return this.Result(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public virtual async Task<IActionResult> Create([FromBody] TModel model, [FromQuery] bool continueEditing)
    {
        if (ModelState.IsValid)
        {
            var entity = model.To<TEntity>();
            await _repository.AddAsync(entity).ConfigureAwait(false);
            await _repository.SaveChangesAsync().ConfigureAwait(false);
            return Ok();
        }
        return BadRequest();
    }

    [HttpGet]
    public virtual IActionResult Edit()
    {
        throw new NotImplementedException();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public virtual IActionResult Edit([FromBody] TModel model, [FromQuery] bool continueEditing)
    {
        throw new NotImplementedException();
    }

    [HttpPost]
    public virtual IActionResult Delete([FromBody] Guid id)
    {
        throw new NotImplementedException();
    }

    [HttpPost]
    public virtual IActionResult DeleteSelected([FromBody] ICollection<Guid> selectedIds)
    {
        throw new NotImplementedException();
    }

    #endregion Create/Edit/Delete

    #region Export/Import

    [HttpGet]
    public virtual IActionResult Export([FromBody] TSearchModel model, [FromQuery] bool all)
    {
        //return File(bytes, MimeTypes.TextXlsx, "categories.xlsx");
        throw new NotImplementedException();
    }

    [HttpPost]
    public virtual IActionResult Import(IFormFile importexcelfile)
    {
        throw new NotImplementedException();
    }

    #endregion Export/Import
}
