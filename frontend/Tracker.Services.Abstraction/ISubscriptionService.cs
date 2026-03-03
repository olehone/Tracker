using Tracker.Domain.Enums;
using Tracker.Domain.Results;

namespace Tracker.Services.Abstraction;

public interface ISubscriptionService
{
    Task<Result<string>> GetCheckoutUrlAsync(SubscriptionPlan plan);
    Task<Result> StopSubscriptionAsync();
}