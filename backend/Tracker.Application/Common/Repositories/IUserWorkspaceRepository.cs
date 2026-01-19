using Tracker.Domain.Entities;
using Tracker.Domain.Enums;

namespace Tracker.Application.Common.Repositories;

public interface IUserWorkspaceRepository : IRepository<UserWorkspace, Guid>
{
    Task<UserWorkspaceRole> GetRoleAsync(Guid userId, Guid workspaceId);
    Task<UserWorkspace?> GetAsync(Guid userId, Guid workspaceId);
}