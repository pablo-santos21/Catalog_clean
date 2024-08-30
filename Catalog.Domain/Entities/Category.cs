using System.Collections.ObjectModel;

namespace Catalog.Domain.Entities;

public partial class Category
{
    public Category()
    {
        Products = new Collection<Product>();
    }

    public int Id { get; set; }
    public string? Name { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? Slug { get; set; }
    public ICollection<Product> Products { get; set; }
}
