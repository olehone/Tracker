using Tracker.Domain.Enums;

namespace Tracker.Domain.Entities;

public class CallUser
{
    public required User User { get; set; }
    public required string ConnectionId { get; set; }
    public required CallUserStatus Status { get; set; }
}