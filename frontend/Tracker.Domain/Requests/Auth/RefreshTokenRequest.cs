namespace Tracker.Domain.Requests;

public class RefreshTokenRequest
{
    public required string RefreshToken { get; set; } = string.Empty;
}
