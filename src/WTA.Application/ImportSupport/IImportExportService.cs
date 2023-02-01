using Microsoft.AspNetCore.Mvc;

namespace WTA.Infrastructure.ExportImport;

public interface IImportExportService
{
    FileContentResult Export<TModel>(List<TModel> list, string? fileName = null);
}
