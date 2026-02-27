using MediatR;
using Tracker.Application.Common.Auth;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Application.UseCases.Boards;
using Tracker.Domain.Dtos;
using Tracker.Domain.Entities;
using Tracker.Domain.Enums;
using Tracker.Domain.Mapping;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Workspaces.GetById;

public sealed class GetWorkspaceByIdQueryHandler(
    IUserContext userContext,
    IUnitOfWorkFactory unitOfWorkFactory)
    : IRequestHandler<GetWorkspaceByIdQuery, Result<WorkspaceFullDto>>
{
    public async Task<Result<WorkspaceFullDto>> Handle(
        GetWorkspaceByIdQuery request,
        CancellationToken cancellationToken)
    {
        await using var uow = unitOfWorkFactory.Create();

        var workspace = await uow.WorkspaceRepository
            .GetByIdAsync(request.Id);
        if (workspace is null)
        {
            return Error.NotFound("Workspace");
        }

        if (userContext.IsUnauthenticated())
        {
            if (WorkspacePolicy.CanAnonView(workspace.Visibility))
            {
                var publicBoards = await uow.BoardRepository.GetPublicByWorkspaceAsync(request.Id);
                return workspace.ToFullDto(WorkspacePermissionsDto.None, publicBoards);
            }

            return AuthErrors.Forbidden("Workspace is private");
        }

        var userId = userContext.GetUserId();
        var user = await uow.UserRepository.GetByIdAsync(userId);
        if (user is null)
        {
            return AuthErrors.Unauthenticated;
        }

        var userRole = user.Role;
        var workspaceRole = await uow.WorkspaceUserRepository
            .GetRoleAsync(userId, workspace.Id);

        if (!WorkspacePolicy.CanView(userRole, workspace.Visibility, workspaceRole))
        {
            return AuthErrors.Forbidden("This workspace is private");
        }

        var boards = await uow.BoardRepository
            .GetByWorkspaceAndUserAsync(request.Id, userId);

        return ToDtoWithParticipating(workspace,
            boards,
            userId,
            userRole,
            workspaceRole);
    }

    public static WorkspaceFullDto ToDtoWithParticipating(Workspace workspace,
        IReadOnlyList<Board> boards,
        Guid userId,
        GlobalRole globalRole,
        WorkspaceUserRole workspaceRole
        )
    {
        return new WorkspaceFullDto
        {
            Id = workspace.Id,
            Title = workspace.Title,
            Description = workspace.Description ?? string.Empty,
            Visibility = workspace.Visibility,
            PermissionRoles = workspace.PermissionRoles,
            Permissions = WorkspacePolicy
                .GetPermissions(workspace.PermissionRoles, workspaceRole, globalRole),
            // Select only boards that user can view
            // Use separate method to not mix permission logic
            Boards = boards
                .Where(b => BoardPolicy.CanView(globalRole, b.Visibility, workspaceRole, b.BoardUsers
                    .FirstOrDefault(ub => ub.UserId == userId)?.Role ?? BoardUserRole.None))
                .Select(b => new BoardSummaryDto
                {
                    Id = b.Id,
                    WorkspaceId = b.WorkspaceId,
                    Title = b.Title,
                    IsArchived = b.ArchiveStatus != ArchiveStatus.NotArchived,
                    ArchiveStatus = b.ArchiveStatus,
                    IsAbleToUnarchive = BoardPolicy.CanChangeArchiveState(globalRole, workspaceRole, b.BoardUsers
                            .FirstOrDefault(ub => ub.UserId == userId)?.Role ?? BoardUserRole.None),
                    IsParticipating = b.BoardUsers.Any(ub => ub.UserId == userId),
                    Visibility = b.Visibility
                })
                .OrderBy(b => b.IsParticipating)
                .ThenByDescending(b => b.Visibility)
                .ToList()
        };
    }
}
