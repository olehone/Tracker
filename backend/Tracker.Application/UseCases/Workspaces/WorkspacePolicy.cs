using Tracker.Domain.Dtos;
using Tracker.Domain.Enums;
using Tracker.Domain.ValueObjects;

namespace Tracker.Application.UseCases.Workspaces;

public static class WorkspacePolicy
{
    public static WorkspacePermissionsDto GetPermissions(WorkspacePermissionRoles permissionRoles,
        WorkspaceUserRole workspaceRole,
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
        WorkspaceUserRole workspaceRole)
    {
        if (globalRole >= GlobalRole.Admin)
        {
            return true;
        }

        if (visibility == WorkspaceVisibility.Public)
        {
            return true;
        }

        if (workspaceRole >= WorkspaceUserRole.None)
        {
            return true;
        }

        return false;
    }

    public static bool CanChangeSettings(GlobalRole globalRole,
        WorkspaceUserRole workspaceRole)
    {
        if (globalRole >= GlobalRole.Admin)
        {
            return true;
        }
        if (workspaceRole >= WorkspaceUserRole.Admin)
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
        WorkspaceUserRole workspaceRole)
    {
        if (globalRole >= GlobalRole.Admin)
        {
            return WorkspacePermissionRole.Owner;
        }

        return MapUserRoleToPermission(workspaceRole);
    }
    private static WorkspacePermissionRole MapUserRoleToPermission(WorkspaceUserRole userWorkspaceRole)
    {
        return userWorkspaceRole switch
        {
            WorkspaceUserRole.Observer => WorkspacePermissionRole.Observer,
            WorkspaceUserRole.Member => WorkspacePermissionRole.Member,
            WorkspaceUserRole.Admin => WorkspacePermissionRole.Admin,
            WorkspaceUserRole.Owner => WorkspacePermissionRole.Owner,
            _ => WorkspacePermissionRole.Any
        };
    }
}