using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using Tracker.Application.Common.Services;
using Tracker.Domain.Enums;
using Tracker.Domain.Options;
using Tracker.Domain.Results;

namespace Tracker.Infrastructure.Stripe;

public class StripeService(IOptions<StripeOptions> options) : IUserSubscriptionService
{
    public async Task<Result<string>> CreateCheckoutSessionAsync(
        Guid userId,
        SubscriptionPlan plan)
    {
        var priceId = MapToPriceId(plan);
        if (priceId is null)
        {
            return Error.Validation("Plan is not supported");
        }

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

    public async Task<Result> CancelSubscriptionAsync(string stripeSubscriptionId)
    {
        var service = new SubscriptionService();
        await service.UpdateAsync(stripeSubscriptionId, new SubscriptionUpdateOptions
        {
            CancelAtPeriodEnd = true
        });
        return Result.Success();
    }

    private string? MapToPriceId(SubscriptionPlan plan)
    {
        if (plan == SubscriptionPlan.Basic)
        {
            return options.Value.BasicSubscriptionName;
        }

        if (plan == SubscriptionPlan.Pro)
        {
            return options.Value.ProSubscriptionName;
        }

        return null;
    }
}
