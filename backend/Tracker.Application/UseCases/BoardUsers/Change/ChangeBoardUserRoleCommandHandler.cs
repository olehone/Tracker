using MediatR;
using Tracker.Application.Common.Auth;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Application.UseCases.Boards;
using Tracker.Domain.Entities;
using Tracker.Domain.Enums;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.BoardUsers.Change;

public class ChangeBoardUserRoleCommandHandler(
    IUserContext userContext,
    IUnitOfWorkFactory unitOfWorkFactory)
    : IRequestHandler<ChangeBoardUserRoleCommand, Result>
{
    public async Task<Result> Handle(
        ChangeBoardUserRoleCommand request,
        CancellationToken cancellationToken)
    {
        await using var uow = unitOfWorkFactory.Create();

        var board = await uow.BoardRepository.GetByIdAsync(request.BoardId);
        if (board is null)
        {
            return Error.NotFound("Board");
        }

        var user = await uow.UserRepository.GetByIdAsync(request.UserId);
        if (user is null)
        {
            return Error.NotFound("User");
        }

        var userId = userContext.GetUserId();
        var userRole = userContext.GetUserRole();
        var workspaceRole = await uow.UserWorkspaceRepository
            .GetRoleAsync(userId, board.WorkspaceId);
        var boardRole = await uow.UserBoardRepository
            .GetRoleAsync(userId, board.Id);

        var permissions = BoardPolicy
            .GetPermissions(board.PermissionRoles, workspaceRole, boardRole, userRole);

        var boardUser = new UserBoard
        {
            UserId = request.UserId,
            BoardId = request.BoardId,
            Role = request.Role,
        };

        if (!BoardPolicy.IsActionAllowed(permissions, BoardAction.ChangeBoard))
        {
            return AuthErrors.Forbidden();
        }

        if (request.Role == UserBoardRole.Owner)
        {
            if (!BoardPolicy.IsActionAllowed(permissions, BoardAction.ChangeOwner))
            {
                return AuthErrors.Forbidden();
            }
            await ChangeOwner(request, uow);
        }
        else
        {
            uow.UserBoardRepository.Update(boardUser);
        }

        var sc = await uow.SaveChangesAsync(cancellationToken);

        return sc.IsFailure
            ? Error.Unknown
            : Result.Success();
    }

    private static async Task ChangeOwner(ChangeBoardUserRoleCommand request, IUnitOfWork uow)
    {
        var oldOwner = await uow.UserBoardRepository.GetOwnerAsync(request.BoardId)!;
        var oldOwnerAsAdmin = new UserBoard
        {
            UserId = oldOwner!.UserId,
            BoardId = oldOwner.BoardId,
            Role = UserBoardRole.Admin,
        };

        var newOwner = new UserBoard
        {
            UserId = request.UserId,
            BoardId = request.BoardId,
            Role = request.Role,
        };
        uow.UserBoardRepository.Update(oldOwnerAsAdmin);
        uow.UserBoardRepository.Update(newOwner);
    }
}