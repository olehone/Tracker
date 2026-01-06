using Tracker.Domain.Entities;
using Tracker.Domain.Enums;

namespace Tracker.Application.Common.Repositories;

public interface IUserWorkspaceRepository : IRepository<UserWorkspace, Guid>
{
    Task<UserWorkspaceRole> GetRole(Guid userId, Guid workspaceId);
    Task<UserWorkspace?> GetByUserAndWorkspace(Guid userId, Guid workspaceId);
}