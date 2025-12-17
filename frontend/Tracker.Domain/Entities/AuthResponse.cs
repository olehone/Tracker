using Tracker.Domain.Dtos;

namespace Tracker.Domain.Entities;

public class AuthResponse
{
    public required UserDto User { get; set; }
    public required RefreshTokenDto RefreshToken { get; set; }
    public required string AccessToken { get; set; }
}
