using WTA.Application.Abstractions.Domain;

namespace WTA.Application.Domain.Blogs;

public class BlogPostTag : BaseEntity, IConcurrencyStamp
{
  public string Name { get; set; } = null!;

  public int BlogPostCount { get; set; }
  public string? ConcurrencyStamp { get; set; }
}
