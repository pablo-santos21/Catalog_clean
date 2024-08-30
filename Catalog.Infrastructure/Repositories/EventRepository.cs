using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
using Catalog.Infrastructure.Context;

namespace Catalog.Infrastructure.Repositories;

public class EventRepository : Repository<ScheduledEvent>, IEventRepository
{
    public EventRepository (ApplicationDbContext context) : base(context)
    {
    }


}
