using MediatR;
using Tracker.Application.Common.Auth;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Boards.Delete;

public class DeleteBoardCommandHandler(
    IUserContext userContext,
    IUnitOfWorkFactory unitOfWorkFactory)
    : IRequestHandler<DeleteBoardCommand, Result>
{
    public async Task<Result> Handle(
        DeleteBoardCommand request,
        CancellationToken cancellationToken)
    {
        if (!userContext.IsAuthenticated())
        {
            return AuthErrors.Unauthenticated;
        }

        await using var uow = unitOfWorkFactory.Create();

        var board = await uow.BoardRepository.GetByIdAsync(request.BoardId);
        if (board is null)
        {
            return Error.NotFound("Board");
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

        var canChange = BoardPolicy.IsActionAllowed(boardPermissions, BoardAction.ChangeBoard);
        if (!canChange)
        {
            return AuthErrors.Forbidden();
        }
        await uow.BoardRepository.RemoveAsync(board.Id);
        
        var sc = await uow.SaveChangesAsync(cancellationToken);
        return sc.IsFailure
            ? Error.Unknown
            : Result.Success();
    }
}