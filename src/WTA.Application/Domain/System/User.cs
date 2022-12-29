using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using WTA.Application.Abstractions.Domain;
using WTA.Application.Domain.Blogs;

namespace WTA.Application.Domain.System;

[SystemModule]
[Permission]
[Display(Name = "用户")]
public class User : BaseEntity, IConcurrencyStamp, IAudit
{
    private enum Permissions
    {
        [Permission(OperationType.Update)]
        [Display(Name = "测试")]
        Test
    }

    [Display(Name = "用户名")]
    [ReadOnly(true)]
    public string? UserName { get; set; }

    [ScaffoldColumn(false)]
    public string? NormalizedUserName { get; set; }

    public string? Email { get; set; }

    [ScaffoldColumn(false)]
    public string? NormalizedEmail { get; set; }

    public bool EmailConfirmed { get; set; }

    [ScaffoldColumn(false)]
    public string? PasswordHash { get; set; }

    [ScaffoldColumn(false)]
    public string? SecurityStamp { get; set; }

    [ScaffoldColumn(false)]
    public string? ConcurrencyStamp { get; set; }

    public string? PhoneNumber { get; set; }
    public bool PhoneNumberConfirmed { get; set; }
    public bool TwoFactorEnabled { get; set; }
    public DateTimeOffset? LockoutEnd { get; set; }
    public bool LockoutEnabled { get; set; }
    public int AccessFailedCount { get; set; }
    public string? Name { get; set; }
    public string? RoleHash { get; set; }
    public string? CreatedBy { get; set; }
    public string? ModifiedBy { get; set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? ModifiedAt { get; set; }
    public List<UserLogin> UserLogins { get; set; } = new List<UserLogin>();
    public List<BlogPost> BlogPosts { get; set; } = new List<BlogPost>();
    public List<BlogComment> BlogComments { get; set; } = new List<BlogComment>();
    public List<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
