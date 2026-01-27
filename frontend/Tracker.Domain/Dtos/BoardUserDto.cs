using Tracker.Domain.Enums;

namespace Tracker.Domain.Dtos;

public class BoardUserDto
{
    public required UserDto User { get; set; }
    public required BoardUserRole Role { get; set; }
}