using Tracker.Domain.Entities.Common;
using Tracker.Domain.Enums;

namespace Tracker.Domain.Entities;

public class UserSubscription : BaseEntity
{
    public required Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public SubscriptionPlan Plan { get; set; } = SubscriptionPlan.Free;
    public DateTimeOffset? CurrentPeriodEnd { get; set; }
    public string? StripeCustomerId { get; set; }
    public string? StripeSubscriptionId { get; set; }
}