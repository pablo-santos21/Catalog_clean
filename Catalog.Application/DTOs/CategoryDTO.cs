namespace Catalog.Application.DTOs;

public class CategoryDTO
{
    public long Id { get; set; }
    public string? Name { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? Slug { get; set; }

    //public ICollection<Product>? Products { get; set; }
}
