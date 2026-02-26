using Tracker.Domain.Dtos;
using Tracker.Domain.Enums;
using Tracker.Domain.ValueObjects;

namespace Tracker.Application.UseCases.Boards;

public static class BoardPolicy
{
    public static BoardPermissionsDto GetPermissions(BoardPermissionRoles permissionRoles,
        WorkspaceUserRole workspaceRole,
        BoardUserRole boardRole,
        GlobalRole globalRole = GlobalRole.None)
    {
        var role = GetEffectivePermission(globalRole, workspaceRole, boardRole);

        return new BoardPermissionsDto
        {
            CanChangeBoard = CanChangeSettings(globalRole, workspaceRole, boardRole),
            CanCreateItem = role >= permissionRoles.MinCreateItemRole,
            CanChangeItem = role >= permissionRoles.MinChangeItemRole,
            CanCreateList = role >= permissionRoles.MinCreateListRole,
            CanChangeList = role >= permissionRoles.MinChangeListRole,
            CanChangeOwner = CanChangeOwner(globalRole, workspaceRole, boardRole),
            CanChangeArchiveStatus = CanChangeArchiveState(globalRole, workspaceRole, boardRole),
            CanDeleteBoard = CanDeleteBoard(globalRole, workspaceRole, boardRole),
        };
    }

    public static bool CanAnonView(BoardVisibility visibility)
    {
        if (visibility <= BoardVisibility.Public)
        {
            return true;
        }
        return false;
    }

    public static bool CanChangeOwner(GlobalRole globalRole,
        WorkspaceUserRole workspaceRole,
        BoardUserRole boardRole)
    {
        if (globalRole >= GlobalRole.Admin ||
            workspaceRole >= WorkspaceUserRole.Admin ||
            boardRole == BoardUserRole.Owner)
        {
            return true;
        }

        return false;
    }

    public static bool CanView(GlobalRole globalRole,
        BoardVisibility visibility,
        WorkspaceUserRole workspaceRole,
        BoardUserRole boardRole)
    {
        if (globalRole >= GlobalRole.Admin || workspaceRole >= WorkspaceUserRole.Admin)
        {
            return true;
        }

        if (visibility <= BoardVisibility.Public)
        {
            return true;
        }

        if (visibility >= BoardVisibility.Private)
        {
            if (boardRole > BoardUserRole.None)
            {
                return true;
            }
            return false;
        }

        if (visibility <= BoardVisibility.Workspace)
        {
            if (workspaceRole > WorkspaceUserRole.None)
            {
                return true;
            }
            return false;
        }
        return false;
    }

    public static bool CanChangeSettings(GlobalRole globalRole,
        WorkspaceUserRole workspaceRole,
        BoardUserRole boardRole)
    {
        return IsHighestPosition(globalRole, workspaceRole, boardRole)
            || workspaceRole >= WorkspaceUserRole.Admin;
    }

    public static bool CanChangeArchiveState(GlobalRole globalRole,
        WorkspaceUserRole workspaceRole,
        BoardUserRole boardRole)
    {
        return IsHighestPosition(globalRole, workspaceRole, boardRole);
    }

    public static bool CanDeleteBoard(GlobalRole globalRole,
        WorkspaceUserRole workspaceRole,
        BoardUserRole boardRole)
    {
        return IsHighestPosition(globalRole, workspaceRole, boardRole);
    }

    private static bool IsHighestPosition(GlobalRole globalRole, WorkspaceUserRole workspaceRole, BoardUserRole boardRole)
    {
        if (globalRole >= GlobalRole.Admin)
        {
            return true;
        }

        if (workspaceRole >= WorkspaceUserRole.Admin)
        {
            return true;
        }

        if (boardRole >= BoardUserRole.Owner)
        {
            return true;
        }

        return false;
    }

    public static bool IsActionAllowed(BoardPermissionsDto permissions,
        BoardAction action)
    {
        return action switch
        {
            BoardAction.CreateItem => permissions.CanCreateItem,
            BoardAction.ChangeItem => permissions.CanChangeItem,
            BoardAction.CreateList => permissions.CanCreateList,
            BoardAction.ChangeList => permissions.CanChangeList,
            BoardAction.ChangeBoard => permissions.CanChangeBoard,
            BoardAction.ChangeOwner => permissions.CanChangeOwner,
            BoardAction.ChangeArchiveStatus => permissions.CanChangeArchiveStatus,
            BoardAction.DeleteBoard => permissions.CanDeleteBoard,
            _ => false
        };
    }

    // Grand global admin or workspace admin same value as board owner
    private static BoardPermissionRole GetEffectivePermission(
        GlobalRole globalRole,
        WorkspaceUserRole workspaceRole,
        BoardUserRole boardRole)
    {
        if (globalRole >= GlobalRole.Admin || workspaceRole >= WorkspaceUserRole.Admin)
        {
            return BoardPermissionRole.Owner;
        }

        return MapUserRoleToPermission(boardRole);
    }
    
    private static BoardPermissionRole MapUserRoleToPermission(BoardUserRole userBoardRole,
        bool isWorkspaceMember = false)
    {
        var boardRole = userBoardRole switch
        {
            BoardUserRole.Observer => BoardPermissionRole.Observer,
            BoardUserRole.Member => BoardPermissionRole.Member,
            BoardUserRole.Admin => BoardPermissionRole.Admin,
            BoardUserRole.Owner => BoardPermissionRole.Owner,
            _ => BoardPermissionRole.Any
        };
        if (boardRole < BoardPermissionRole.WorkspaceMember &&
            isWorkspaceMember)
        {
            return BoardPermissionRole.WorkspaceMember;
        }

        return boardRole;
    }
}