using MediatR;
using Tracker.Application.Common.Auth;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Domain.Dtos;
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
            .GetByIdWithBoardsAsync(request.Id);
        if (workspace is null)
        {
            return Error.NotFound("Workspace");
        }

        if (!userContext.IsAuthenticated())
        {
            if (WorkspacePolicy.CanAnonView(workspace.Visibility))
            {
                return workspace.ToFullDto(WorkspacePermissionsDto.None);
            }
            return AuthErrors.Forbidden();
        }

        var userId = userContext.GetUserId();
        var userRole = userContext.GetUserRole();
        var workspaceRole = await uow.UserWorkspaceRepository
            .GetRole(userId, workspace.Id);

        if (WorkspacePolicy.CanView(userRole, workspace.Visibility, workspaceRole))
        {
            return AuthErrors.Forbidden("You are not member of this workspace");
        }
        var workspacePolicy = WorkspacePolicy
            .GetPermissions(workspace.PermissionRoles, workspaceRole, userRole);

        return workspace.ToFullDto(workspacePolicy);
    }
}
