using MediatR;
using Tracker.Application.Common.Auth;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Application.UseCases.Workspaces;
using Tracker.Domain.Dtos;
using Tracker.Domain.Entities;
using Tracker.Domain.Enums;
using Tracker.Domain.Mapping;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Boards.Create;

public sealed class CreateBoardCommandHandler(
    IUserContext userContext,
    IUnitOfWorkFactory unitOfWorkFactory)
    : IRequestHandler<CreateBoardCommand, Result<BoardSummaryDto>>
{
    private const WorkspaceAction action = WorkspaceAction.CreateBoard;

    public async Task<Result<BoardSummaryDto>> Handle(
        CreateBoardCommand request,
        CancellationToken cancellationToken)
    {
        if (!userContext.IsAuthenticated())
        {
            return AuthErrors.Unauthenticated;
        }

        await using var uow = unitOfWorkFactory.Create();

        var workspace = await uow.WorkspaceRepository.GetByIdAsync(request.WorkspaceId);

        if (workspace is null)
        {
            return Error.NotFound("Workspace");
        }

        var userId = userContext.GetUserId();
        var userRole = userContext.GetUserRole();
        var workspaceRole = await uow.UserWorkspaceRepository
            .GetRole(userId, request.WorkspaceId);

        var permissions = WorkspacePolicy
            .GetPermissions(workspace.PermissionRoles, workspaceRole, userRole);

        if (WorkspacePolicy.IsActionAllowed(permissions, action))
        {
            return await Create(userId, request, uow, cancellationToken);
        }

        return AuthErrors.Forbidden();
    }

    private static async Task<Result<BoardSummaryDto>> Create(Guid userId,
        CreateBoardCommand request,
        IUnitOfWork uow,
        CancellationToken cancellationToken)
    {
        var board = new Board
        {
            WorkspaceId = request.WorkspaceId,
            Title = request.Title,
            Description = request.Description
        };

        var userBoard = new UserBoard
        {
            UserId = userId,
            BoardId = board.Id,
            Role = UserBoardRole.Owner
        };

        await uow.BoardRepository.AddAsync(board);
        await uow.UserBoardRepository.AddAsync(userBoard);

        var sc = await uow.SaveChangesAsync(cancellationToken);

        return sc.IsFailure
            ? Error.Unknown
            : board.ToSummaryDto();
    }
}
