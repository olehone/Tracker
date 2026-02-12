using System;
using MediatR;
using Tracker.Application.Common.Auth;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Domain.Entities;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Boards;

public static class BoardHelper
{
    public static async Task<Result<Board>> GetBoardForActionAsync(IUnitOfWork uow,
        IUserContext userContext, Guid boardId, BoardAction action)
    {
        if (userContext.IsUnauthenticated())
        {
            return AuthErrors.Unauthenticated;
        }

        var board = await uow.BoardRepository.GetByIdAsync(boardId);

        if (board is null)
        {
            return Error.NotFound("Board");
        }

        var isAllowed = await IsActionAllowedAsync(uow, userContext, board, action);
        if (!isAllowed)
        {
            return AuthErrors.Forbidden("You cannot change this item");
        }
        return board;
    }

    public static async Task<Result<BoardList>> GetBoardListForActionAsync(IUnitOfWork uow,
        IUserContext userContext, Guid boardListId, BoardAction action, Guid boardId)
    {
        if (userContext.IsUnauthenticated())
        {
            return AuthErrors.Unauthenticated;
        }

        var board = await uow.BoardRepository.GetWithWorkspaceByListAsync(boardListId);
        if (board is null)
        {
            return Error.NotFound("Board");
        }

        if (board.Id != boardId)
        {
            return Error.Validation("Board does not have this list");
        }

        var isAllowed = await IsActionAllowedAsync(uow, userContext, board, action);
        if (!isAllowed)
        {
            return AuthErrors.Forbidden("You cannot change this list");
        }

        var boardList = await uow.BoardListRepository.GetByIdAsync(boardListId);
        if (boardList is null)
        {
            return Error.NotFound("Board list");
        }

        return boardList;
    }

    public static async Task<Result<BoardItem>> GetBoardItemForActionAsync(IUnitOfWork uow,
        IUserContext userContext, Guid boardItemId, Guid? boardId = null)
    {
        BoardAction action = BoardAction.ChangeItem;

        if (userContext.IsUnauthenticated())
        {
            return AuthErrors.Unauthenticated;
        }

        var board = await uow.BoardRepository.GetWithWorkspaceByItemAsync(boardItemId);
        if (board is null)
        {
            return Error.NotFound("Board");
        }

        if (boardId is not null && board.Id != boardId)
        {
            return Error.Validation("Board does not have this item");
        }

        var boardItem = await uow.BoardItemRepository.GetByIdAsync(boardItemId);
        if (boardItem is null)
        {
            return Error.NotFound("Board item");
        }

        var isAllowed = await IsActionAllowedAsync(uow, userContext, board, action);
        var assigned = boardItem.Assignees
            .Any(bia => bia.BoardUser.UserId == userContext.GetUserId());

        if (!isAllowed && !assigned)
        {
            return AuthErrors.Forbidden("You cannot change this item");
        }

        return boardItem;
    }

    public static async Task<Result<BoardItem>> GetItemAsync(IUnitOfWork uow,
        IUserContext userContext, Guid boardItemId)
    {
        var item = await uow.BoardItemRepository.GetByIdAsync(boardItemId);
        if (item is null)
        {
            return Error.NotFound("Item");
        }

        var board = await uow.BoardRepository.GetWithWorkspaceByItemAsync(boardItemId);
        if (board is null)
        {
            return Error.NotFound("Board", "Item");
        }

        if (userContext.IsUnauthenticated())
        {
            if (!BoardPolicy.CanAnonView(board.Visibility))
            {
                return AuthErrors.Unauthenticated;
            }
            return item;
        }

        var userId = userContext.GetUserId();
        var user = await uow.UserRepository.GetByIdAsync(userId);
        if (user is null)
        {
            return Error.NotFound("Item");
        }
        var workspaceRole = await uow.WorkspaceUserRepository.GetRoleAsync(userId, board.WorkspaceId);
        var boardRole = await uow.BoardUserRepository.GetRoleAsync(userId, board.Id);
        var permissions = BoardPolicy
            .GetPermissions(board.PermissionRoles, workspaceRole, boardRole, user.Role);

        return BoardPolicy.CanView(user.Role, board.Visibility, workspaceRole, boardRole)
            ? item
            : AuthErrors.Forbidden("Board is private");
    }

    public static async Task<Result<BoardItemAttachment>> GetItemAttachmentForActionAsync(IUnitOfWork uow,
        IUserContext userContext, Guid attachmentId)
    {
        var attachment = await uow.BoardItemAttachmentRepository.GetByIdAsync(attachmentId);
        if (attachment is null)
        {
            return Error.NotFound("Attachment");
        }
        if (attachment.IsDeleted)
        {
            return Error.Gone("Attachment");
        }

        BoardAction action = BoardAction.ChangeItem;

        if (userContext.IsUnauthenticated())
        {
            return AuthErrors.Unauthenticated;
        }

        var board = await uow.BoardRepository.GetWithWorkspaceByItemAttachmentAsync(attachmentId);
        if (board is null)
        {
            return Error.NotFound("Board", "attachment");
        }

        var boardItem = await uow.BoardItemRepository.GetByIdAsync(attachment.BoardItemId);
        if (boardItem is null)
        {
            return Error.NotFound("Board item", "attachment");
        }

        var isAllowed = await IsActionAllowedAsync(uow, userContext, board, action);
        var assigned = boardItem.Assignees
            .Any(bia => bia.BoardUser.UserId == userContext.GetUserId());

        if (!isAllowed && !assigned)
        {
            return AuthErrors.Forbidden("You cannot change this item");
        }

        return attachment;
    }

    // User must be authenticated before call
    // for proper separation of unauthenticated and forbidden error
    private static async Task<bool> IsActionAllowedAsync(IUnitOfWork uow,
        IUserContext userContext, Board board, BoardAction action)
    {
        var userId = userContext.GetUserId();
        var user = await uow.UserRepository.GetByIdAsync(userId);
        if (user is null)
        {
            return false;
        }

        var workspaceRole = await uow.WorkspaceUserRepository.GetRoleAsync(userId, board.WorkspaceId);
        var boardRole = await uow.BoardUserRepository.GetRoleAsync(userId, board.Id);
        var permissions = BoardPolicy
            .GetPermissions(board.PermissionRoles, workspaceRole, boardRole, user.Role);

        return BoardPolicy.IsActionAllowed(permissions, action);
    }
}