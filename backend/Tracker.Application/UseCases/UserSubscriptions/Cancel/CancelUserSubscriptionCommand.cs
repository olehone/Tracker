using MediatR;

namespace Tracker.Application.UseCases.UserSubscriptions.Cancel;

public class CancelUserSubscriptionCommand : IRequest
{
    public required string StripeSubscriptionId { get; set; }
}