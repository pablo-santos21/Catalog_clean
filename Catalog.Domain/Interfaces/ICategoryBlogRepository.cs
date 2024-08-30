using Catalog.Domain.Entities;
using Catalog.Domain.Entities.Pagination;

namespace Catalog.Domain.Interfaces;

public interface ICategoryBlogRepository : IRepository<CategoryBlog>
{
    Task<PagedResult<CategoryBlog>> GetAllPostsCategoriesAsync(int page, int pageSize);
    Task<CategoryBlog> GetCategoryAsync(string name);
}
