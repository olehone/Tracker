namespace Tracker.Application.Common.Services;

public interface IUserSubscriptionService
{
    Task<string> CreateCheckoutSessionAsync(Guid userId, string priceId);
}
