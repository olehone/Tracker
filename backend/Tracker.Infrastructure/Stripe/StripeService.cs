using Microsoft.Extensions.Options;
using Stripe.Checkout;
using Tracker.Application.Common.Services;
using Tracker.Domain.Options;

namespace Tracker.Infrastructure.Stripe;

public class StripeService(IOptions<StripeOptions> options) : IUserSubscriptionService
{
    public async Task<string> CreateCheckoutSessionAsync(
        Guid userId,
        string priceId)
    {
        var sessionOptions = new SessionCreateOptions
        {
            Mode = "subscription",
            LineItems =
            [
                new() { Price = priceId, Quantity = 1 }
            ],
            SuccessUrl = options.Value.SuccessUrl,
            CancelUrl = options.Value.CancelUrl,
            Metadata = new Dictionary<string, string>
            {
                { "userId", userId.ToString() }
            }
        };

        var service = new SessionService();
        var session = await service.CreateAsync(sessionOptions);
        return session.Url;
    }
}
