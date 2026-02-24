using Tracker.Domain.Enums;

namespace Tracker.Domain.Dtos;

public class CallUserDto
{
    public required UserDto User { get; set; }
    public required string ConnectionId { get; set; }
    public required CallUserStatus Status { get; set; }
}
