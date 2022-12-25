using System.ComponentModel.DataAnnotations;

namespace WTA.Web.Models;

[Display]
public class TestModel
{
  [Display]
  [Required]
  public string? UserName { get; set; }
}
