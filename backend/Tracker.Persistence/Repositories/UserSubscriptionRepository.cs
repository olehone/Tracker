using Tracker.Application.Common.Repositories;
using Tracker.Domain.Entities;

namespace Tracker.Persistence.Repositories;

internal class UserSubscriptionRepository(ApplicationDbContext dbContext)
    : Repository<UserSubscription, Guid>(dbContext), IUserSubscriptionRepository
{
}