namespace Tracker.Domain.Dtos;

public class UserDto
{
    public required Guid Id { get; set; }
    public required string Email { get; set; }
    public required string Username { get; set; }
    // Should I show other users Role status?
    public required string Role { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
}
