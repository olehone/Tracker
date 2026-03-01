using Tracker.Domain.Enums;
using Tracker.Domain.Results;
using Tracker.Services.Abstraction;
using Tracker.Services.Abstraction.Results;
using Tracker.Services.ApiClients;

namespace Tracker.Services;

public class SubscriptionService(IApiErrorHandler apiErrorHandler,
    ISubscriptionApi api) : ISubscriptionService
{
    public Task<Result<string>> GetCheckoutUrlAsync(SubscriptionPlan plan)
    {
        return apiErrorHandler.ExecuteAsync(() => api.GetCheckoutUrlAsync(plan));
    }
    public Task<Result> StopSubscriptionAsync()
    {
        return apiErrorHandler.ExecuteAsync(api.StopSubscriptionAsync);
    }

}
