using Catalog.Domain.Entities;
using Catalog.Domain.Entities.Pagination;

namespace Catalog.Domain.Interfaces;

public interface ITypeEventRepository : IRepository<TypeEvent>
{
    Task<PagedResult<TypeEvent>> GetAllEventTypeAsync(int page, int pageSize);
    Task<TypeEvent> GetTypeAsync(string name);
}
