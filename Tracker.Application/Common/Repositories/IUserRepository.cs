
using Tracker.Application.Results;
using Tracker.Domain.Entities;

namespace Tracker.Application.Common.Repositories;

public interface IUserRepository : IRepository<User, Guid>
{
    Task<bool> EmailExistsAsync(string email);
    Task<bool> UsernameExistsAsync(string username);
    Task<User?> GetByEmailAsync(string email);
}
