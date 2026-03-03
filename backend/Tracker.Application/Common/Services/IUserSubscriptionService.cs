using Tracker.Domain.Enums;
using Tracker.Domain.Results;

namespace Tracker.Application.Common.Services;

public interface IUserSubscriptionService
{
    Task<Result<string>> CreateCheckoutSessionAsync(Guid userId, SubscriptionPlan plan);
    Task<Result> CancelSubscriptionAsync(string stripeSubscriptionId);
}
