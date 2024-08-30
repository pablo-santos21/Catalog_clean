
using Catalog.Domain.Entities.Pagination;
using System.Linq.Expressions;

namespace Catalog.Domain.Interfaces;

public interface IRepository<T>
{
    Task<PagedResult<T>> GetAllAsync(int page, int pageSize);
    Task<T> GetAsync(Expression<Func<T, bool>>predicate);
    Task<T> CreateAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task<T> DeleteAsync(T entity);
}
