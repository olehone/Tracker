using MediatR;
using Tracker.Application.Common.Auth;
using Tracker.Application.Common.Services;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.UserSubscriptions.CreateSession;

public class CreateCheckoutSessionCommandHandler(
    IUnitOfWorkFactory unitOfWorkFactory,
    IUserContext userContext,
    IUserSubscriptionService stripeService
) : IRequestHandler<CreateCheckoutSessionCommand, Result<string>>
{
    public async Task<Result<string>> Handle(CreateCheckoutSessionCommand request, CancellationToken ct)
    {
        if (userContext.IsUnauthenticated())
        {
            return AuthErrors.Unauthenticated;
        }

        await using var uow = unitOfWorkFactory.Create();

        var userId = userContext.GetUserId();
        var user = await uow.UserRepository.GetByIdAsync(userId);
        if (user is null)
        {
            return Error.NotFound("User");
        }

        return await stripeService.CreateCheckoutSessionAsync(user.Id, request.Plan);
    }
}