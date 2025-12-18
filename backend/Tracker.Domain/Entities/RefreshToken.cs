using Tracker.Domain.Entities.Common;

namespace Tracker.Domain.Entities;

public class RefreshToken: BaseEntity
{
    public string Token { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public DateTimeOffset ExpiresAt { get; set; }
    public bool IsActive => DateTimeOffset.UtcNow < ExpiresAt;
}
