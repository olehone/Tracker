using Tracker.Domain.Entities;
using Tracker.Domain.Enums;

namespace Tracker.Application.Common.Repositories;

public interface IWorkspaceUserRepository : IRepository<WorkspaceUser, Guid>
{
    Task<WorkspaceUserRole> GetRoleAsync(Guid userId, Guid workspaceId);
    Task<WorkspaceUser?> GetAsync(Guid userId, Guid workspaceId);
    Task<IReadOnlyList<WorkspaceUser>> GetAsync(Guid workspaceId);
    Task<WorkspaceUser?> GetOwnerAsync(Guid workspaceId);
}