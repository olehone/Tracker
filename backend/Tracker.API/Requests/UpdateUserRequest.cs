namespace Tracker.API.Requests;

public sealed class UpdateUserRequest
{
    public required string Username { get; set; }
    public required string FirstName { get; set; }
    public string? LastName { get; set; }
}