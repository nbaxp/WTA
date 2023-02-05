using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using WTA.Application.Abstractions.Components;

namespace WTA.Infrastructure.ExportImport;

[Service<IImportExportService>]
public class ClosedXmlImportExportService : IImportExportService
{
    private readonly IStringLocalizer _localizer;

    public ClosedXmlImportExportService(IStringLocalizer localizer)
    {
        _localizer = localizer;
    }

    public virtual FileContentResult Export<TModel>(List<TModel> list, string? fileName = null)
    {
        var modelType = typeof(TModel);
        var name = fileName ?? _localizer.GetString(modelType.GetCustomAttribute<DisplayAttribute>()?.Name ?? modelType.Name);
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add(name);
        this.AddListToSheet(worksheet, list);
        worksheet.ColumnsUsed().AdjustToContents();
        //foreach (var column in worksheet.ColumnsUsed())
        //{
        //    column.Width = column.Cells().Max(cell => cell.Value.ToString().Length);
        //}
        worksheet.RowsUsed().AdjustToContents();
        using var ms = new MemoryStream();
        workbook.SaveAs(ms);
        return new FileContentResult(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
        {
            FileDownloadName = $"{name}.xlsx"
        };
    }

    protected virtual void AddListToSheet<TModel>(IXLWorksheet worksheet, List<TModel> list)
    {
        var text = "测1a";
        for (int i = 0; i < 5; i++)
        {
            text += text;
            var cell = worksheet.Cell(i + 1, 1);
            cell.SetValue(text);
            this.SetCellStyle(cell);
        }
    }

    protected virtual void SetCellStyle(IXLCell cell, bool isHeader = false)
    {
        cell.Style.Font.SetFontName("宋体");
        //cell.Style.Font.FontSize = 12;
        cell.Style.Font.Bold = isHeader;

        cell.Style.Border.TopBorder =
        cell.Style.Border.RightBorder =
        cell.Style.Border.BottomBorder =
        cell.Style.Border.LeftBorder = XLBorderStyleValues.Thick;

        cell.Style.Border.TopBorderColor =
        cell.Style.Border.RightBorderColor =
        cell.Style.Border.BottomBorderColor =
        cell.Style.Border.LeftBorderColor = XLColor.Black;
    }
}
