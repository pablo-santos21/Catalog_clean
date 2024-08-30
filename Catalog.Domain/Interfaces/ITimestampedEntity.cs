namespace Catalog.Domain.Interfaces;

public interface ITimestampedEntity
{
    DateTime CreatedAt { get; set; }
    DateTime UpdateAt { get; set; }
}
