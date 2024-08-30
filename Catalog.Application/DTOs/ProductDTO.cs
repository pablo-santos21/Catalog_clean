namespace Catalog.Application.DTOs;

public class ProductDTO
{
    public long Id { get; set; }

    public string? Name { get; set; }

    public string Description { get; set; } = string.Empty;
    public string? Slug { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public List<string>? Image { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdateAt { get; set; }
    public string? UserID { get; set; }
    public int CategoryId { get; set; }
  
}
