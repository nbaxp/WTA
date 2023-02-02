using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WTA.Infrastructure.ExportImport;

[Route("[controller]/[action]")]
[ApiExplorerSettings(GroupName = "test")]
public class ImportExportController : Controller
{
    private readonly IImportExportService _importExport;

    public ImportExportController(IImportExportService importSupport)
    {
        _importExport = importSupport;
    }

    [HttpGet]
    public IActionResult Export()
    {
        return _importExport.Export(new List<TestModel> { new TestModel() });
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    public IActionResult Import([Required] IFormFile file)
    {
        return Json(file.ContentType);
    }
}

public class TestModel
{
    public string Name { get; set; } = Guid.NewGuid().ToString();
}
