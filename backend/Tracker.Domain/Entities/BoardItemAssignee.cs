using Tracker.Domain.Entities.Common;

namespace Tracker.Domain.Entities;

public class BoardItemAssignee : BaseEntity
{
    public required Guid BoardUserId { get; set; }
    public required Guid BoardItemId { get; set; }
    public UserBoard BoardUser { get; set; } = null!;
    public BoardItem Item { get; set; } = null!;
}
