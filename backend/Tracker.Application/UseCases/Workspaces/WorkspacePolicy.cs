using Tracker.Domain.Dtos;
using Tracker.Domain.Enums;
using Tracker.Domain.ValueObjects;

namespace Tracker.Application.UseCases.Workspaces;

public static class WorkspacePolicy
{
    public static WorkspacePermissionsDto GetPermissions(WorkspacePermissionRoles permissionRoles,
        UserWorkspaceRole workspaceRole,
        GlobalRole globalRole = GlobalRole.None)
    {
        var role = GetEffectivePermission(globalRole, workspaceRole);
        return new WorkspacePermissionsDto()
        {
            CanCreateBoard = role >= permissionRoles.MinCreateBoardRole,
            CanChangeBoard = role >= permissionRoles.MinChangeBoardRole,
            CanChangeWorkspace = CanChangeSettings(globalRole, workspaceRole),
        };
    }

    public static bool CanAnonView(WorkspaceVisibility visibility)
    {
        if (visibility <= WorkspaceVisibility.Public)
        {
            return true;
        }
        return false;
    }
    public static bool CanView(GlobalRole globalRole,
        WorkspaceVisibility visibility,
        UserWorkspaceRole workspaceRole)
    {
        if (globalRole >= GlobalRole.Admin)
        {
            return true;
        }

        if (visibility == WorkspaceVisibility.Public)
        {
            return true;
        }

        if (workspaceRole >= UserWorkspaceRole.None)
        {
            return true;
        }

        return false;
    }

    public static bool CanChangeSettings(GlobalRole globalRole,
        UserWorkspaceRole workspaceRole)
    {
        if (globalRole >= GlobalRole.Admin)
        {
            return true;
        }
        if (workspaceRole >= UserWorkspaceRole.Admin)
        {
            return true;
        }
        return false;
    }

    public static bool IsActionAllowed(WorkspacePermissionsDto permissions,
        WorkspaceAction action)
    {
        return action switch
        {
            WorkspaceAction.ChangeBoard => permissions.CanChangeBoard,
            WorkspaceAction.CreateBoard => permissions.CanCreateBoard,
            WorkspaceAction.ChangeWorkspace => permissions.CanChangeWorkspace,
            _ => false,
        };
    }

    // Grand global admin same value as workspace owner
    private static WorkspacePermissionRole GetEffectivePermission(
        GlobalRole globalRole,
        UserWorkspaceRole workspaceRole)
    {
        if (globalRole >= GlobalRole.Admin)
        {
            return WorkspacePermissionRole.Owner;
        }

        return MapUserRoleToPermission(workspaceRole);
    }
    private static WorkspacePermissionRole MapUserRoleToPermission(UserWorkspaceRole userWorkspaceRole)
    {
        return userWorkspaceRole switch
        {
            UserWorkspaceRole.Observer => WorkspacePermissionRole.Observer,
            UserWorkspaceRole.Member => WorkspacePermissionRole.Member,
            UserWorkspaceRole.Admin => WorkspacePermissionRole.Admin,
            UserWorkspaceRole.Owner => WorkspacePermissionRole.Owner,
            _ => WorkspacePermissionRole.Any
        };
    }
}