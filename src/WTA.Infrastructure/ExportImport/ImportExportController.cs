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
}

public class TestModel
{
    public string Name { get; set; } = Guid.NewGuid().ToString();
}
