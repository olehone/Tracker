using Microsoft.EntityFrameworkCore;
using Tracker.Application.Common.Repositories;
using Tracker.Domain.Dtos;
using Tracker.Domain.Entities;
using Tracker.Domain.Mapping;

namespace Tracker.Persistence.Repositories;

public class UserRepository : Repository<User, Guid>, IUserRepository
{

    public UserRepository(ApplicationDbContext applicationDbContext)
        : base(applicationDbContext)
    {
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _dbSet
            .AnyAsync(user => user.Email == email);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _dbSet
            .FirstOrDefaultAsync(user => user.Email == email);
    }

    public async Task<List<UserDto>> SearchByUsernamePartAsync(string username, int skip, int take)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentException("Cannot search user by empty username");
        }
        return await _dbSet
            .Where(u => u.Username.StartsWith(username))
            .OrderBy(u => u.Username)
            .Skip(skip)
            .Take(take)
            .Select(u => u.ToDto())
            .ToListAsync();
    }

    public async Task<bool> UsernameExistsAsync(string username)
    {
        return await _dbSet
            .AnyAsync(user => user.Username == username);
    }
}
