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
        return await _dbSet
            .AsNoTracking()
            .AnyAsync(user => user.Email == email);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(u => u.Subscription)
            .FirstOrDefaultAsync(user => user.Email == email);
    }

    private IQueryable<User> ApplyUsernameFilter(string? username)
    {
        var query = _dbSet.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(username))
        {
            query = query.Where(u =>
                EF.Functions.Like(u.Username, $"%{username}%"));
        }

        return query;
    }

    public async Task<int> CountAsync(string? username)
    {
        return await ApplyUsernameFilter(username)
            .CountAsync();
    }

    public async Task<List<User>> GetAsync(
        string? username,
        int skip,
        int take)
    {
        return await ApplyUsernameFilter(username)
            .OrderBy(u => u.Username)
            .Include(u => u.Subscription)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task<bool> UsernameExistsAsync(string username)
    {
        return await _dbSet
            .AsNoTracking()
            .AnyAsync(user => user.Username == username);
    }
}