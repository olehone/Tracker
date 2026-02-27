using Tracker.Application.Common.Auth;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Domain.Entities;
using Tracker.Domain.Enums;
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

        if (IsArchiveStatusBlocking(board) && action != BoardAction.ChangeArchiveStatus)
        {
            return ArchiveErrors.Archived("Board");
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

        if (IsArchiveStatusBlocking(board))
        {
            return ArchiveErrors.Archived("Board");
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
        var action = BoardAction.ChangeItem;

        if (userContext.IsUnauthenticated())
        {
            return AuthErrors.Unauthenticated;
        }

        var board = await uow.BoardRepository.GetWithWorkspaceByItemAsync(boardItemId);
        if (board is null)
        {
            return Error.NotFound("Board");
        }

        if (IsArchiveStatusBlocking(board))
        {
            return ArchiveErrors.Archived("Board");
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

        if (IsArchiveStatusBlocking(board))
        {
            return ArchiveErrors.Archived("Board");
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

        return BoardPolicy.CanView(user.Role, board.Visibility, workspaceRole, boardRole)
            ? item
            : AuthErrors.Forbidden("Board is private");
    }

    public static async Task<Result> CanViewBoardAsync(Board board, IUnitOfWork uow, IUserContext userContext,
        CancellationToken cancellationToken)
    {
        if (IsArchiveStatusBlocking(board))
        {
            return ArchiveErrors.Archived("Board");
        }

        if (userContext.IsUnauthenticated())
        {
            if (BoardPolicy.CanAnonView(board.Visibility))
            {
                return AuthErrors.Forbidden("Board is private");
            }
        }

        var userId = userContext.GetUserId();
        var user = await uow.UserRepository.GetByIdAsync(userId);
        if (user is null)
        {
            return AuthErrors.Unauthenticated;
        }

        var userRole = user.Role;

        var workspaceRole = await uow.WorkspaceUserRepository
            .GetRoleAsync(userId, board.WorkspaceId);
        var boardRole = await uow.BoardUserRepository
            .GetRoleAsync(userId, board.Id);

        if (!BoardPolicy.CanView(userRole, board.Visibility, workspaceRole, boardRole))
        {
            return AuthErrors.Forbidden("Board is private");
        }

        return Result.Success();
    }

    public static async Task<Result<ItemComment>> GetItemCommentForActionAsync(IUnitOfWork uow,
        IUserContext userContext, Guid commentId)
    {
        var comment = await uow.ItemCommentRepository.GetByIdAsync(commentId);
        if (comment is null)
        {
            return Error.NotFound("Comment");
        }

        if (comment.IsDeleted)
        {
            return Error.Gone("Comment");
        }

        var userId = userContext.GetUserId();
        var ownComment = comment.UploadedBy.Id == userId;

        var item = await GetBoardItemForActionAsync(uow, userContext, comment.BoardItemId);

        if (item.IsSuccess)
        {
            return comment;
        }

        if (ArchiveErrors.IsArchived(item.Error))
        {
            return item.Error;
        }

        if (item.Error.Type == ErrorType.Forbidden
            && ownComment)
        {
            return comment;
        }

        return item.Error;
    }

    public static bool IsArchiveStatusBlocking(Board board)
    {
        return board.ArchiveStatus != ArchiveStatus.NotArchived;
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