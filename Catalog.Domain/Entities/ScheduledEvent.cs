using Catalog.Domain.Enums;
using Catalog.Domain.Interfaces;

namespace Catalog.Domain.Entities;

public partial class ScheduledEvent : ITimestampedEntity
{
    public long Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Local { get; set; }
    public string? City { get; set; }
    public States State { get; set; } = States.RS;
    public string? Neighborhood { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdateAt { get; set; }
    public DateTime? EventDate { get; set; }
    public bool Occurred { get; set; }
    public string? Slug { get; set; }

    public int TypeEventId { get; set; }
    public TypeEvent? TypeEvent { get; set; }
}
