using System.ComponentModel.DataAnnotations;
using WTA.Application.Abstractions.Domain;
using WTA.Application.Domain.System;

namespace WTA.Application.Domain.Blogs;

[SystemModule]
[Permission]
[Display(Name = "评论")]
public class BlogComment : BaseEntity, ITenant
{
    public Guid UserId { get; set; }
    public Guid BlogPostId { get; set; }
    public Guid? ParentId { get; set; }

    public string CommentText { get; set; } = null!;
    public DateTimeOffset CreatedOn { get; set; }

    public bool IsApproved { get; set; }
    public User User { get; set; } = null!;
    public BlogPost BlogPost { get; set; } = null!;
    public BlogComment? Parent { get; set; }
    public List<BlogComment> Children { get; set; } = new List<BlogComment>();
    public string? Tenant { get; set; }
}
