using Microsoft.Extensions.DependencyInjection;

namespace WTA.Infrastructure.ExportImport;

public static class ImportExportExtensions
{
    public static void AddImportExport(this IServiceCollection services)
    {
        services.AddScoped<IImportExportService, ClosedXMLImportService>();
    }

    public static void AddImportExport<T>(this IServiceCollection services) where T : class, IImportExportService
    {
        services.AddScoped<IImportExportService, T>();
    }
}
