using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
using Catalog.Infrastructure.Context;

namespace Catalog.Infrastructure.Repositories;

public class PostRepository : Repository<PostBlog>, IPostRepository
{
    public PostRepository (ApplicationDbContext context) : base(context)
    {

    }


}
