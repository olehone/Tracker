namespace Tracker.Domain.Dtos;

public class UserDto
{
    public required Guid Id { get; set; }
    public required string Email { get; set; }
    public required string Username { get; set; }
    public required string Role { get; set; }
    public required string FirstName { get; set; }
    public string? LastName { get; set; }
}
