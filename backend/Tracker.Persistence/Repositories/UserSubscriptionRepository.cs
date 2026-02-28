using Microsoft.EntityFrameworkCore;
using Tracker.Application.Common.Repositories;
using Tracker.Domain.Entities;

namespace Tracker.Persistence.Repositories;

internal class UserSubscriptionRepository(ApplicationDbContext dbContext)
    : Repository<UserSubscription, Guid>(dbContext), IUserSubscriptionRepository
{
    public Task<UserSubscription?> GetByUserIdAsync(Guid userId)
    {
        return _dbSet
            .FirstOrDefaultAsync(s => s.UserId == userId);
    }
    public Task<UserSubscription?> GetBySubscriptionIdAsync(string subscriptionId)
    {
        return _dbSet
            .FirstOrDefaultAsync(s => s.StripeSubscriptionId == subscriptionId);
    }
}