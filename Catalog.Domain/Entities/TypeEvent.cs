using System.Collections.ObjectModel;

namespace Catalog.Domain.Entities;

public partial class TypeEvent
{
    public TypeEvent()
    {
        ScheduledEvents = new Collection<ScheduledEvent>();
    }

    public int Id { get; set; }
    public string? Name { get; set; }

    public ICollection<ScheduledEvent>? ScheduledEvents { get; set; }

}
