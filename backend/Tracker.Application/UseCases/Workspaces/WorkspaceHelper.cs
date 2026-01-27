using Tracker.Application.Common.Auth;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Domain.Entities;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Workspaces;

public static class WorkspaceHelper
{
    public static async Task<Result<Workspace>> GetWorkspaceForActionAsync(IUnitOfWork uow, 
        IUserContext userContext, Guid workspaceId, WorkspaceAction action)
    {
        if (userContext.IsUnauthenticated())
        {
            return AuthErrors.Unauthenticated;
        }

        var workspace = await uow.WorkspaceRepository.GetByIdAsync(workspaceId);

        if (workspace is null)
        {
            return Error.NotFound("Workspace");
        }

        var isAllowed = await IsActionAllowed(uow, userContext, workspace, action);
        if (!isAllowed)
        {
            return AuthErrors.Forbidden();
        }
        return workspace;
    }

    // User must be authenticated before call
    // for proper separation of unauthenticated and forbidden error
    private static async Task<bool> IsActionAllowed(IUnitOfWork uow, 
        IUserContext userContext, Workspace workspace, WorkspaceAction action)
    {
        var userId = userContext.GetUserId();
        var userRole = userContext.GetUserRole();
        var workspaceRole = await uow.WorkspaceUserRepository.GetRoleAsync(userId, workspace.Id);
        var permissions = WorkspacePolicy
            .GetPermissions(workspace.PermissionRoles, workspaceRole, userRole);

        return WorkspacePolicy.IsActionAllowed(permissions, action);
    }
}