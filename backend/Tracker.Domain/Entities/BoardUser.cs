using Tracker.Domain.Entities.Common;
using Tracker.Domain.Enums;

namespace Tracker.Domain.Entities;

public class BoardUser : BaseEntity
{
    public required Guid UserId { get; set; }
    public required Guid BoardId { get; set; }
    public required BoardUserRole Role { get; set; }
    public User User { get; set; } = null!;
    public Board Board { get; set; } = null!;
    public List<BoardItemAssignee> AssignedItems { get; set; } = [];
}
