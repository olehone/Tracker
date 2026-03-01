using MediatR;
using Tracker.Application.Common.Services;

namespace Tracker.Application.UseCases.UserSubscriptions.CreateSession;

public class CreateCheckoutSessionCommandHandler(
    IUserSubscriptionService stripeService,
) : IRequestHandler<CreateCheckoutSessionCommand, string>
{
    public Task<string> Handle(CreateCheckoutSessionCommand request, CancellationToken ct)
    {
        return stripeService.CreateCheckoutSessionAsync(request.UserId, request.PriceId);
    }
}