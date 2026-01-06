using System;
using Tracker.Domain.Entities;
using Tracker.Domain.Enums;
using Tracker.Domain.ValueObjects;

namespace Tracker.Application.UseCases.Workspaces;

public static class WorkspacePolicy
{
    public static bool IsActionAllowedGlobalRole(GlobalRole globalRole, 
        WorkspaceAction workspaceAction)
    {
        if (globalRole >= GlobalRole.Admin)
        {
            return true;
        }
        return false;
    }

    public static bool IsActionAllowedAnonymous(WorkspaceSettings workspaceSettings, 
        WorkspaceAction action)
    {
        if (action == WorkspaceAction.ViewWorkspace &&
        workspaceSettings.Visibility == WorkspaceVisibility.Public)
        {
            return true;
        }
        var minRole = MinRoleForAction(workspaceSettings, action);
        return minRole == WorkspacePermissionRole.Any;
    }

    public static bool IsActionAllowed(UserWorkspace workspaceMembership,
        WorkspaceSettings workspaceSettings,
        WorkspaceAction action)
    {
        if (action == WorkspaceAction.ViewWorkspace)
        {
            return true;
        }

        var minRole = MinRoleForAction(workspaceSettings, action);
        var userRole = MapUserRoleToPermission(workspaceMembership?.Role);

        return userRole >= minRole;
    }

    private static WorkspacePermissionRole MinRoleForAction(WorkspaceSettings workspaceSettings,
        WorkspaceAction action)
    {
        return action switch
        {
            WorkspaceAction.CreateBoard => workspaceSettings.MinCreateBoardRole,
            WorkspaceAction.ChangeBoard => workspaceSettings.MinChangeBoardRole,
            _ => WorkspacePermissionRole.None
        };
    }

    private static WorkspacePermissionRole MapUserRoleToPermission(UserWorkspaceRole? userWorkspaceRole)
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