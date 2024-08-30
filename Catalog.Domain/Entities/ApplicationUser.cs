using Catalog.Domain.Interfaces;
using System.Collections.ObjectModel;

namespace Catalog.Domain.Entities;

public class ApplicationUser : IApplicationUser
{
    public ApplicationUser()
    {
        Products = new Collection<Product>();
        Posts = new Collection<PostBlog>();
    }


    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }

    public ICollection<Product> Products { get; set; }
    public ICollection<PostBlog> Posts { get; set; }
}
