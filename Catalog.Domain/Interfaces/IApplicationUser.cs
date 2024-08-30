using Catalog.Domain.Entities;


namespace Catalog.Domain.Interfaces;

public interface IApplicationUser
{
    string? RefreshToken { get; set; }
    DateTime RefreshTokenExpiryTime { get; set; }

    
    ICollection<Product> Products { get; set; }
    ICollection<PostBlog> Posts { get; set; }
}
