using Tracker.Application.Common.Repositories;
using Tracker.Domain.Entities;

namespace Tracker.Persistence.Repositories;

public class UserRepository : Repository<User, Guid>, IUserRepository
{

    public UserRepository(ApplicationDbContext applicationDbContext)
        : base(applicationDbContext)
    {
    }

}
