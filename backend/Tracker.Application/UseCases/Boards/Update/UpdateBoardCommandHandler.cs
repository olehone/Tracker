using MediatR;
using Tracker.Application.Common.Auth;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Application.UseCases.Workspaces;
using Tracker.Domain.Entities;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Boards.Update;

public class UpdateBoardCommandHandler(
    IUserContext userContext,
    IUnitOfWorkFactory unitOfWorkFactory)
    : IRequestHandler<UpdateBoardCommand, Result>
{
    public async Task<Result> Handle(UpdateBoardCommand request,
        CancellationToken cancellationToken)
    {
        if (!userContext.IsAuthenticated())
        {
            return AuthErrors.Unauthenticated;
        }

        await using var uow = unitOfWorkFactory.Create();
        var board = await uow.BoardRepository
            .GetByIdAsync(request.BoardId);

        if (board is null)
        {
            return Error.NotFound("Board");
        }
        var workspace= await uow.WorkspaceRepository
            .GetByIdAsync(board.WorkspaceId);

        if (workspace is null)
        {
            return Error.NotFound("Workspace", "board");
        }

        var userId = userContext.GetUserId();
        var userRole = userContext.GetUserRole();
        var workspaceRole = await uow.UserWorkspaceRepository
            .GetRole(userId, board.WorkspaceId);
        var boardRole = await uow.UserBoardRepository
            .GetRole(userId, request.BoardId);
        
        var workspacePermissions = WorkspacePolicy
            .GetPermissions(workspace.PermissionRoles, workspaceRole, userRole);

        var canChange = BoardPolicy
            .CanChangeSettings(userRole, workspaceRole, boardRole, workspacePermissions);
        if (!canChange)
        {
            return AuthErrors.Forbidden();
        }

        var newBoard = new Board
        {
            Id = request.BoardId,
            WorkspaceId = board.WorkspaceId,
            Title = request.Title,
            Description = request.Description,
            Visibility = request.Visibility,
            PermissionRoles = request.PermissionRoles,
        };

        uow.BoardRepository.Update(newBoard);
        var result = await uow.SaveChangesAsync(cancellationToken);
        if (result.IsFailure)
        {
            return result;
        }
        return Result.Success();
    }
}