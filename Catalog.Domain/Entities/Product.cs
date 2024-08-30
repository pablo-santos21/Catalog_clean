using Catalog.Domain.Interfaces;

namespace Catalog.Domain.Entities;

public partial class Product : ITimestampedEntity
{
    public long Id { get; set; }
    public string? Name { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public List<string>? Image { get; set; }
    public bool IsActive { get; set; }
    public string? Slug { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdateAt { get; set; }

    public int CategoryId { get; set; }
    public Category? Category { get; set; }

    public string? UserID { get; set; }
    public ApplicationUser? User { get; set; }
}
