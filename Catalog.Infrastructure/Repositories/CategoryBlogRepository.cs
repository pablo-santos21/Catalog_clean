using Catalog.Domain.Entities;
using Catalog.Domain.Entities.Pagination;
using Catalog.Domain.Interfaces;
using Catalog.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Repositories;

public class CategoryBlogRepository : Repository<CategoryBlog>, ICategoryBlogRepository
{
    public CategoryBlogRepository (ApplicationDbContext context) : base(context)
    {

    }

    public async Task<PagedResult<CategoryBlog>> GetAllPostsCategoriesAsync(int page, int pageSize)
    {
        var categories = await _context.CategoriesBlog.Skip((page - 1) * pageSize).Take(pageSize).AsNoTracking().Include(p => p.PostsBlog).ToListAsync();

        var count = await _context.CategoriesBlog.CountAsync();
        var totalPages = (int)Math.Ceiling(count / (double)pageSize);

        return new PagedResult<CategoryBlog>(count, page, totalPages, categories);
    }

    public async Task<CategoryBlog> GetCategoryAsync(string name)
    {
        var category = await _context.CategoriesBlog.AsNoTracking().Include(p => p.PostsBlog).FirstOrDefaultAsync(c => c.Name == name);

        return category;
    }
}
