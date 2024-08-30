using Catalog.Domain.Entities;
using Catalog.Domain.Entities.Pagination;

namespace Catalog.Domain.Interfaces;

public interface ICategoryRepository : IRepository<Category>
{
    Task<PagedResult<Category>> GetAllProductsCategoriesAsync(int page, int pageSize);
    Task<Category> GetCategoryAsync(string name);

}
