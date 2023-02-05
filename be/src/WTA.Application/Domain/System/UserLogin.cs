using System.ComponentModel.DataAnnotations;
using WTA.Application.Abstractions.Domain;

namespace WTA.Application.Domain.System;

[SystemModule]
[Permission]
[Display(Name = "三方登录")]
public class UserLogin : BaseEntity
{
    public string LoginProvider { get; set; } = null!;
    public string ProviderKey { get; set; } = null!;
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}
