using MediatR;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Domain.Enums;

namespace Tracker.Application.UseCases.UserSubscriptions.Cancel;

public class CancelUserSubscriptionCommandHandler(IUnitOfWorkFactory unitOfWorkFactory)
    : IRequestHandler<CancelUserSubscriptionCommand>
{
    public async Task Handle(
        CancelUserSubscriptionCommand request,
        CancellationToken cancellationToken)
    {
        await using var uow = unitOfWorkFactory.Create();

        var subscription = await uow.UserSubscriptionRepository
            .GetBySubscriptionIdAsync(request.StripeSubscriptionId);

        if (subscription is null)
        {
            return;
        }

        subscription.Plan = SubscriptionPlan.Free;
        subscription.CurrentPeriodEnd = DateTimeOffset.UtcNow;

        uow.UserSubscriptionRepository.Update(subscription);
        await uow.SaveChangesAsync(cancellationToken);
    }
}