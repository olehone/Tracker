using Tracker.Domain.Entities;

namespace Tracker.Application.Common.Repositories;

public interface IUserSubscriptionRepository : IRepository<UserSubscription, Guid>
{
    Task<UserSubscription?> GetByUserIdAsync(Guid userId);
    Task<UserSubscription?> GetBySubscriptionIdAsync(string subscriptionId);
}
