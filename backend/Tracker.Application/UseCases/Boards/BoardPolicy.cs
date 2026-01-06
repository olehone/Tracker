using Tracker.Domain.Dtos;
using Tracker.Domain.Enums;
using Tracker.Domain.ValueObjects;

namespace Tracker.Application.UseCases.Boards;

public static class BoardPolicy
{
    public static BoardPermissionsDto GetPermissions(BoardPermissionRoles permissionRoles,
        UserWorkspaceRole workspaceRole,
        UserBoardRole boardRole,
        GlobalRole globalRole = GlobalRole.None)
    {
        if (globalRole >= GlobalRole.Admin ||
            workspaceRole >= UserWorkspaceRole.Admin)
        {
            return BoardPermissionsDto.All;
        }

        return new BoardPermissionsDto()
        {
            CanCreateItem = permissionRoles.MinCreateItemRole >= MapUserRoleToPermission(boardRole),
            CanChangeItem = permissionRoles.MinChangeItemRole >= MapUserRoleToPermission(boardRole),
            CanCreateList = permissionRoles.MinCreateListRole >= MapUserRoleToPermission(boardRole),
            CanChangeList = permissionRoles.MinChangeListRole >= MapUserRoleToPermission(boardRole),
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

    public static bool CanView(GlobalRole globalRole,
        BoardVisibility visibility,
        UserWorkspaceRole workspaceRole,
        UserBoardRole boardRole)
    {
        if (globalRole >= GlobalRole.Admin)
        {
            return true;
        }

        if (visibility <= BoardVisibility.Public)
        {
            return true;
        }

        if (visibility >= BoardVisibility.Private)
        {
            if (boardRole > UserBoardRole.None)
            {
                return true;
            }
            return false;
        }

        if (visibility <= BoardVisibility.Workspace)
        {
            if (workspaceRole > UserWorkspaceRole.None)
            {
                return true;
            }
            return false;
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
            _ => false
        };
    }

    private static BoardPermissionRole MapUserRoleToPermission(UserBoardRole userBoardRole,
        bool isWorkspaceMember = false)
    {
        var boardRole = userBoardRole switch
        {
            UserBoardRole.Observer => BoardPermissionRole.Observer,
            UserBoardRole.Member => BoardPermissionRole.Member,
            UserBoardRole.Admin => BoardPermissionRole.Admin,
            UserBoardRole.Owner => BoardPermissionRole.Owner,
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