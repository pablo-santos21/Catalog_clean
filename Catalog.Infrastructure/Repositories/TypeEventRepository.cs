using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
using Catalog.Domain.Entities.Pagination;
using Catalog.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Repositories;


public class TypeEventRepository : Repository<TypeEvent>, ITypeEventRepository
{
    public TypeEventRepository(ApplicationDbContext context) : base(context)
    {

    }

    public async Task<PagedResult<TypeEvent>> GetAllEventTypeAsync(int page, int pageSize)
    {
        var eventsType = await _context.TypesEvent.Skip((page - 1) * pageSize).Take(pageSize).AsNoTracking().Include(e => e.ScheduledEvents).ToListAsync();

        var count = await _context.TypesEvent.CountAsync();
        var totalPages = (int)Math.Ceiling(count / (double)pageSize);

        return new PagedResult<TypeEvent>(count, page, totalPages, eventsType); ;
    }

    public async Task<TypeEvent> GetTypeAsync(string name)
    {
        var type = await _context.TypesEvent.AsNoTracking().Include(p => p.ScheduledEvents).FirstOrDefaultAsync(c => c.Name == name);

        return type;
    }
}
