using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
using Catalog.Domain.Entities.Pagination;
using Catalog.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Repositories;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    
    public CategoryRepository(ApplicationDbContext context) : base(context)
    {
      
    }

    //public async Task<IEnumerable<Category>> GetAllProductsCategoriesAsync()
    //{
    //    var categories = await _context.Categories.AsNoTracking().Include(p => p.Products).ToListAsync();

    //    return categories;
    //}   

    public async Task<PagedResult<Category>> GetAllProductsCategoriesAsync(int page, int pageSize)
    {
        var categories = await _context.Categories.Skip((page - 1) * pageSize).Take(pageSize).OrderBy(c => c.Id).Include(p => p.Products).ToListAsync();

        var count = await _context.Categories.CountAsync();
        var totalPages = (int)Math.Ceiling(count / (double)pageSize);

        return new PagedResult<Category>(count, page, totalPages, categories);
     
    }

    public async Task<Category> GetCategoryAsync(string name)
    {
        var category = await _context.Categories.AsNoTracking().Include(p => p.Products).FirstOrDefaultAsync(c => c.Name == name);

        return category;
    }

}
