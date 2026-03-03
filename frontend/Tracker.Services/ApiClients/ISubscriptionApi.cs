using Refit;
using Tracker.Domain.Enums;

namespace Tracker.Services.ApiClients;

public interface ISubscriptionApi
{
    [Post("/api/subscription")]
    Task<IApiResponse<string>> GetCheckoutUrlAsync([Query] SubscriptionPlan plan);
    [Delete("/api/subscription")]
    Task<IApiResponse> StopSubscriptionAsync();
}
