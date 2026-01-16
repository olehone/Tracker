using MediatR;
using Tracker.Application.Common.Auth;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Application.UseCases.Boards;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.BoardItems.Update;

public class UpdateBoardItemCommandHandler(
    IUserContext userContext,
    IUnitOfWorkFactory unitOfWorkFactory)
    : IRequestHandler<UpdateBoardItemCommand, Result>
{
    public async Task<Result> Handle(UpdateBoardItemCommand request,
        CancellationToken cancellationToken)
    {
        if (!userContext.IsAuthenticated())
        {
            return AuthErrors.Unauthenticated;
        }
        await using var uow = unitOfWorkFactory.Create();
        var boardItem = await uow.BoardItemRepository.GetByIdAsync(request.BoardItemId);
        if (boardItem is null)
        {
            return Error.NotFound("Board item");
        }

        var boardList = await uow.BoardListRepository.GetByIdAsync(boardItem.BoardListId);
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

        var canChange = BoardPolicy.IsActionAllowed(boardPermissions, BoardAction.ChangeItem);
        if (!canChange)
        {
            return AuthErrors.Forbidden();
        }

        boardItem.Title = request.Title;
        boardItem.Description = request.Description;

        uow.BoardItemRepository.Update(boardItem);
        var result = await uow.SaveChangesAsync(cancellationToken);
        if (result.IsFailure)
        {
            return result;
        }
        return Result.Success();
    }
}