using Tracker.Domain.Entities;

namespace Tracker.Application.Common.Repositories;

public interface IUserWorkspaceRepository : IRepository<UserWorkspace, Guid>
{
    Task<UserWorkspace?> GetByUserAndWorkspaceIds(Guid userId, Guid workspaceId);
}