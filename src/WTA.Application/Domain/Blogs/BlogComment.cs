using WTA.Application.Core;
using WTA.Application.Domain.Users;

namespace WTA.Application.Domain.Blogs;

public class BlogComment : BaseEntity
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
}
