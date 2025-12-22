using Tracker.Domain.Entities.Common;

namespace Tracker.Domain.Entities;

public class RefreshToken : BaseEntity
{
    public required string Token { get; set; } = string.Empty;
    public required Guid UserId { get; init; }
    public User? User { get; set; }
    public required DateTimeOffset ExpiresAt { get; set; }
    public bool IsActive => DateTimeOffset.UtcNow < ExpiresAt;
}
