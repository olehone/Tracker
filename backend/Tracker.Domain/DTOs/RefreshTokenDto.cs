using Tracker.Domain.Entities;

namespace Tracker.Domain.DTOs;

public class RefreshTokenDto
{
    public required string Token {get; set;}
    public required DateTimeOffset ExpiresAt {get; set;}
    public required bool IsActive { get; set; }
}
