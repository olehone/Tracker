namespace Tracker.Domain.Dtos;

public class CallDto
{
    public required Guid Id { get; set; }
    public required DateTimeOffset StartedAt { get; set; }
    public required List<UserDto> Users { get; set; }
}
