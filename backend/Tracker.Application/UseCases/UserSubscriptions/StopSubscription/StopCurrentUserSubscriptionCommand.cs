using MediatR;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.UserSubscriptions.StopSubscription;

public class StopCurrentUserSubscriptionCommand : IRequest<Result>
{
}
