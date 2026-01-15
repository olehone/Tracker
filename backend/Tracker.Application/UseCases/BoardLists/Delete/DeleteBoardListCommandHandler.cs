using MediatR;
using Tracker.Application.Common.Auth;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Application.UseCases.Boards;
using Tracker.Domain.Enums;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.BoardLists.Delete;

public class DeleteBoardListCommandHandler(
    IUserContext userContext,
    IUnitOfWorkFactory unitOfWorkFactory)
    : IRequestHandler<DeleteBoardListCommand, Result>
{
    public async Task<Result> Handle(
        DeleteBoardListCommand request,
        CancellationToken cancellationToken)
    {
        if (!userContext.IsAuthenticated())
        {
            return AuthErrors.Unauthenticated;
        }

        await using var uow = unitOfWorkFactory.Create();
        var boardList = await uow.BoardListRepository.GetByIdAsync(request.BoardListId);

        if (boardList is null)
        {
            return Error.NotFound("Board list");
        }

        var board = await uow.BoardRepository.GetByIdAsync(boardList.BoardId);
        if (board is null)
        {
            return Error.NotFound("Board", "board list");
        }

        var workspace = await uow.WorkspaceRepository
            .GetByIdAsync(board.WorkspaceId);
        if (workspace is null)
        {
            return Error.NotFound("Workspace", "board");
        }

        var userId = userContext.GetUserId();
        var userRole = userContext.GetUserRole();
        var workspaceRole = await uow.UserWorkspaceRepository
            .GetRoleAsync(userId, workspace.Id);
        var boardRole = await uow.UserBoardRepository
            .GetRoleAsync(userId, board.Id);

        var boardPermissions = BoardPolicy
            .GetPermissions(board.PermissionRoles, workspaceRole, boardRole, userRole);

        var canChange = BoardPolicy.IsActionAllowed(boardPermissions, BoardAction.ChangeList);
        if (!canChange)
        {
            return AuthErrors.Forbidden();
        }

        await uow.BoardListRepository.RemoveAsync(boardList.Id);

        return Result.Success();
    }
}