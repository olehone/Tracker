using MediatR;
using Tracker.Application.Common.Auth;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Application.UseCases.Boards;
using Tracker.Application.UseCases.BoardUsers.Change;
using Tracker.Domain.Entities;
using Tracker.Domain.Enums;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.BoardUsers.Remove;

public class RemoveUserFromBoardCommandHandler(
    IUserContext userContext,
    IUnitOfWorkFactory unitOfWorkFactory)
    : IRequestHandler<RemoveUserFromBoardCommand, Result>
{
    public async Task<Result> Handle(
        RemoveUserFromBoardCommand request,
        CancellationToken cancellationToken)
    {
        await using var uow = unitOfWorkFactory.Create();

        var board = await uow.BoardRepository
            .GetByIdAsync(request.BoardId);
        if (board is null)
        {
            return Error.NotFound("Board");
        }

        var userBoard = await uow.UserBoardRepository
            .GetAsync(request.UserId, request.BoardId);
        if (userBoard is null)
        {
            return Error.NotFound("User", "board");
        }

        var userId = userContext.GetUserId();
        var userRole = userContext.GetUserRole();
        var workspaceRole = await uow.UserWorkspaceRepository
            .GetRoleAsync(userId, board.WorkspaceId);
        var boardRole = await uow.UserBoardRepository
            .GetRoleAsync(userId, board.Id);

        var permissions = BoardPolicy
            .GetPermissions(board.PermissionRoles, workspaceRole, boardRole, userRole);


        if (!BoardPolicy.IsActionAllowed(permissions, BoardAction.ChangeBoard))
        {
            return AuthErrors.Forbidden();
        }

        if (userBoard.Role == UserBoardRole.Owner && userBoard.UserId == userId)
        {
            await uow.BoardRepository.RemoveAsync(board.Id);
        }
        else
        {
            await uow.UserBoardRepository.RemoveAsync(userBoard.Id);
        }

        var sc = await uow.SaveChangesAsync(cancellationToken);

        return sc.IsFailure
            ? Error.Unknown
            : Result.Success();
    }

}