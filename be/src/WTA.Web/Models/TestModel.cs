using System.ComponentModel.DataAnnotations;
using WTA.Application.Abstractions.Application;

namespace WTA.Web.Models;

[Display]
public class TestModel
{
    [Display]
    [Required]
    [OperatorType(OperatorType.Contains)]
    [OperatorType(OperatorType.EndsWith)]
    [OperatorType(OperatorType.StartsWith)]
    [OperatorType(OperatorType.Equal)]
    public string? UserName { get; set; }

    [OperatorType(OperatorType.OrderBy)]
    public string? OrderBy { get; set; }

    [OperatorType(OperatorType.Equal)]
    public bool EmailConfirmed { get; set; }
}
