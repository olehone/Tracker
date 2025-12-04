namespace Tracker.Domain.Entities.Common;
internal class BaseEntity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
}
