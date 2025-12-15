
using Microsoft.EntityFrameworkCore;

using Tracker.Application.Common.Repositories;
using Tracker.Domain.Entities;

namespace Tracker.Persistence.Repositories;

public class UserRepository : Repository<User, Guid>, IUserRepository
{

    public UserRepository(ApplicationDbContext applicationDbContext)
        : base(applicationDbContext)
    {
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _dbSet.AnyAsync(user => user.Email == email);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _dbSet.FirstOrDefaultAsync(user => user.Email == email);
    }

    public async Task<bool> UsernameExistsAsync(string username)
    {
        return await _dbSet.AnyAsync(user => user.Username == username);
    }
}
