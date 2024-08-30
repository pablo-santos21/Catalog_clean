using System.Collections.ObjectModel;

namespace Catalog.Domain.Entities;

public partial class CategoryBlog
{
    public CategoryBlog()
    {
        PostsBlog = new Collection<PostBlog>();
    }

    public int Id { get; set; }
    public string? Name { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? Slug { get; set; }
    public ICollection<PostBlog> PostsBlog { get; set; }
}
