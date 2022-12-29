using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WTA.Application.Services.Users;

[Display(Name = "登录")]
public class LoginModel
{
    [StringLength(20, MinimumLength = 5)]
    [Description]
    [Required]
    [Display]
    public string UserName { get; set; } = null!;

    [Required]
    [DataType(DataType.Password)]
    [Display]
    public string Password { get; set; } = null!;
}
