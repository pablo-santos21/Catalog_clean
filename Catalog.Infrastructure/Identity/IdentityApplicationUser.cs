using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.Collections.ObjectModel;


namespace Catalog.Infrastructure.Identity;

public class IdentityApplicationUser : IdentityUser, IApplicationUser
{
    public IdentityApplicationUser()
    {
        Products = new Collection<Product>();
        Posts = new Collection<PostBlog>();
    }

    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }

    public ICollection<Product> Products { get; set; }
    public ICollection<PostBlog> Posts { get; set; }
}
