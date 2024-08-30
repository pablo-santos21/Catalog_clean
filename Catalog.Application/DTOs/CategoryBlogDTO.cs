namespace Catalog.Application.DTOs;

public class CategoryBlogDTO
{
    public long Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Slug { get; set; }

    //public ICollection<PostBlog>? PostsBlog { get; set; }
}
