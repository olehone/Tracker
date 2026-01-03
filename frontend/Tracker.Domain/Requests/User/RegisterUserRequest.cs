namespace Tracker.Domain.Requests;

public class RegisterUserRequest
{
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string Username { get; set; }
    public required string FirstName { get; set; }
    public string LastName { get; set; } = string.Empty;
}
