using Catalog.Domain.Interfaces;

namespace Catalog.Domain.Entities;

public partial class PostBlog : ITimestampedEntity
{
    public long Id { get; set; }
    public string? Title { get; set; }
    public string? SubTitle { get; set; }
    public string? Context { get; set; }
    public bool IsApproved { get; set; }
    public bool IsDeleted { get; set; } = false;
    public string? ImagePost { get; set; }
    public string? Slug { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdateAt { get; set; }
    public List<string>? Tags { get; set; }

    public string? UserID { get; set; }
    public ApplicationUser? User { get; set; }

    public int CategoryBlogId { get; set; }
    public CategoryBlog? CategoryBlog { get; set; }
}
