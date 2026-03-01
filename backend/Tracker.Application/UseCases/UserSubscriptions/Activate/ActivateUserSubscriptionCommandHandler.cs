using MediatR;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Domain.Entities;

namespace Tracker.Application.UseCases.UserSubscriptions.Activate;

public class ActivateUserSubscriptionCommandHandler(IUnitOfWorkFactory unitOfWorkFactory)
    : IRequestHandler<ActivateUserSubscriptionCommand>
{
    public async Task Handle(
        ActivateUserSubscriptionCommand request,
        CancellationToken cancellationToken)
    {
        await using var uow = unitOfWorkFactory.Create();

        var subscription = await uow.UserSubscriptionRepository
            .GetByUserIdAsync(request.UserId);

        var newSubscription = new UserSubscription
        {
            Id = subscription is null ? Guid.NewGuid() : subscription.Id,
            UserId = request.UserId,
            Plan = request.Plan,
            CurrentPeriodEnd = request.CurrentPeriodEnd,
            StripeCustomerId = request.StripeCustomerId,
            StripeSubscriptionId = request.StripeSubscriptionId,

        };

        if (subscription is null)
        {
            await uow.UserSubscriptionRepository.AddAsync(newSubscription);
        }
        else
        {
            uow.UserSubscriptionRepository.Update(newSubscription);
        }

        await uow.SaveChangesAsync(cancellationToken);
    }
}