using MediatR;
using Tracker.Application.Common.Auth;
using Tracker.Application.Common.Services;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.UserSubscriptions.StopSubscription;

public class StopCurrentUserSubscriptionCommandHandler(
    IUnitOfWorkFactory unitOfWorkFactory,
    IUserContext userContext,
    IUserSubscriptionService subscriptionService)
    : IRequestHandler<StopCurrentUserSubscriptionCommand, Result>
{
    public async Task<Result> Handle(
        StopCurrentUserSubscriptionCommand request,
        CancellationToken cancellationToken)
    {
        if (userContext.IsUnauthenticated())
        {
            return AuthErrors.Unauthenticated;
        }

        var userId = userContext.GetUserId();

        await using var uow = unitOfWorkFactory.Create();

        var subscription = await uow.UserSubscriptionRepository
            .GetByUserIdAsync(userId);

        if (subscription is null)
        {
            return Error.NotFound("Subscription", "user");
        }

        return await subscriptionService.CancelSubscriptionAsync(subscription.StripeSubscriptionId!);
    }
}