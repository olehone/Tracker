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

        var isAllowed = await IsActionAllowed(uow, userContext, board, action);
        if (!isAllowed)
        {
            return AuthErrors.Forbidden();
        }
        return board;
    }

    public static async Task<Result<BoardList>> GetBoardListForActionAsync(IUnitOfWork uow, 
        IUserContext userContext, Guid boardListId, BoardAction action)
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

        //if (board.Id != boardId)
        //{
        //    return Error.Validation("Board does not have this list");
        //}

        var isAllowed = await IsActionAllowed(uow, userContext, board, action);
        if (!isAllowed)
        {
            return AuthErrors.Forbidden();
        }

        var boardList = await uow.BoardListRepository.GetByIdAsync(boardListId);
        if (boardList is null)
        {
            return Error.NotFound("Board list");
        }

        return boardList;
    }
    
    public static async Task<Result<BoardItem>> GetBoardItemForActionAsync(IUnitOfWork uow, 
        IUserContext userContext, Guid boardItemId, BoardAction action, Guid boardId)
    {
        if (userContext.IsUnauthenticated())
        {
            return AuthErrors.Unauthenticated;
        }

        var board = await uow.BoardRepository.GetWithWorkspaceByItemAsync(boardItemId);
        if (board is null)
        {
            return Error.NotFound("Board");
        }

        if (board.Id != boardId)
        {
            return Error.Validation("Board does not have this item");
        }

        var isAllowed = await IsActionAllowed(uow, userContext, board, action);
        if (!isAllowed)
        {
            return AuthErrors.Forbidden();
        }

        var boardItem = await uow.BoardItemRepository.GetByIdAsync(boardItemId);
        if (boardItem is null)
        {
            return Error.NotFound("Board item");
        }

        return boardItem;
    }

    // User must be authenticated before call
    // for proper separation of unauthenticated and forbidden error
    private static async Task<bool> IsActionAllowed(IUnitOfWork uow, 
        IUserContext userContext, Board board, BoardAction action)
    {
        var userId = userContext.GetUserId();
        var userRole = userContext.GetUserRole();
        var workspaceRole = await uow.UserWorkspaceRepository.GetRoleAsync(userId, board.WorkspaceId);
        var boardRole = await uow.UserBoardRepository.GetRoleAsync(userId, board.Id);
        var permissions = BoardPolicy
            .GetPermissions(board.PermissionRoles, workspaceRole, boardRole, userRole);

        return BoardPolicy.IsActionAllowed(permissions, action);
    }
}