using Catalog.Domain.Interfaces;
using Catalog.Infrastructure.Context;
using Catalog.Domain.Entities.Pagination;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Catalog.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly ApplicationDbContext _context;
    public Repository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<T>> GetAllAsync(int page, int pageSize)
    {
        var query = await _context.Set<T>().Skip((page - 1) * pageSize).Take(pageSize).AsNoTracking().ToListAsync();

        var count = await _context.Set<T>().CountAsync();
        var totalPages = (int)Math.Ceiling(count / (double)pageSize);

        return new PagedResult<T>(count, page, totalPages, query);
    }

    public async Task<T?> GetAsync(Expression<Func<T, bool>> predicate)
    {
        return await _context.Set<T>().FirstOrDefaultAsync(predicate);
    }

    public async Task<T> CreateAsync(T entity)
    {
        await _context.Set<T>().AddAsync(entity);
        await _context.SaveChangesAsync();

        return entity;
    }

    public async Task<T> UpdateAsync(T entity)
    {
         _context.Set<T>().Update(entity);
        await _context.SaveChangesAsync();

        return entity;
    }

    public async Task<T> DeleteAsync(T entity)
    {
        _context.Set<T>().Remove(entity);
        await _context.SaveChangesAsync();

        return entity;
    }
        
}
