using MediatR;
using Tracker.Domain.Enums;

namespace Tracker.Application.UseCases.UserSubscriptions.Update;

public class UpdateUserSubscriptionCommand : IRequest
{
    public required string StripeSubscriptionId { get; set; }
    public required SubscriptionPlan Plan { get; set; }
    public required DateTimeOffset CurrentPeriodEnd { get; set; }
}
