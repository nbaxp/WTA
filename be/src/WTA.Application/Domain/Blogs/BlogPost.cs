using System.ComponentModel.DataAnnotations;
using WTA.Application.Abstractions.Domain;
using WTA.Application.Domain.System;

namespace WTA.Application.Domain.Blogs;

[SystemModule]
[Permission]
[Display(Name = "帖子")]
public class BlogPost : BaseEntity, ITenant
{
    public Guid UserId { get; set; }
    public string? MetaKeywords { get; set; }
    public string? MetaDescription { get; set; }
    public string? MetaTitle { get; set; }
    public string Title { get; set; } = null!;
    public string Body { get; set; } = null!;
    public string? BodyOverview { get; set; }
    public bool AllowComments { get; set; }
    public string? Tags { get; set; }
    public DateTimeOffset CreatedOn { get; set; }
    public User User { get; set; } = null!;
    public List<BlogComment> BlogCommnets { get; set; } = new List<BlogComment>();
    public string? Tenant { get; set; }
}
