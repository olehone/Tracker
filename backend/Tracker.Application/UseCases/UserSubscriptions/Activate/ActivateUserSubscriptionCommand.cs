using MediatR;
using Tracker.Domain.Enums;

namespace Tracker.Application.UseCases.UserSubscriptions.Activate;

public class ActivateUserSubscriptionCommand : IRequest
{
    public required Guid UserId { get; set; }
    public required string StripeCustomerId { get; set; }
    public required string StripeSubscriptionId { get; set; }
    public required SubscriptionPlan Plan { get; set; }
    public required DateTimeOffset CurrentPeriodEnd { get; set; }
}