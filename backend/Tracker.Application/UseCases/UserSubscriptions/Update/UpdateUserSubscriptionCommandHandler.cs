using MediatR;
using Tracker.Application.Common.UnitOfWork;

namespace Tracker.Application.UseCases.UserSubscriptions.Update;

public class UpdateUserSubscriptionCommandHandler(IUnitOfWorkFactory unitOfWorkFactory)
    : IRequestHandler<UpdateUserSubscriptionCommand>
{
    public async Task Handle(
        UpdateUserSubscriptionCommand request,
        CancellationToken cancellationToken)
    {
        await using var uow = unitOfWorkFactory.Create();

        var subscription = await uow.UserSubscriptionRepository
            .GetBySubscriptionIdAsync(request.StripeSubscriptionId);

        if (subscription is null)
        {
            return;
        }

        subscription.Plan = request.Plan;
        subscription.CurrentPeriodEnd = request.CurrentPeriodEnd;
        subscription.StripeSubscriptionId = request.StripeSubscriptionId;

        uow.UserSubscriptionRepository.Update(subscription);
        await uow.SaveChangesAsync(cancellationToken);
    }
}