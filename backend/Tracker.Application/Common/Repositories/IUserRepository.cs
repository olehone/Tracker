using Tracker.Domain.Entities;
using Tracker.Domain.Dtos;

namespace Tracker.Application.Common.Repositories;

public interface IUserRepository : IRepository<User, Guid>
{
    Task<bool> EmailExistsAsync(string email);
    Task<bool> UsernameExistsAsync(string username);
    Task<List<UserDto>> SearchUsersByUsernameAsync(string username, int skip, int take);
    Task<User?> GetByEmailAsync(string email);
}
