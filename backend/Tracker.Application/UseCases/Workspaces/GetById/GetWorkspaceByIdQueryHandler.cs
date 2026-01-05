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
    : IRequestHandler<GetWorkspaceByIdQuery, Result<WorkspaceDto>>
{
    private const WorkspaceAction action = WorkspaceAction.ViewWorkspace;

    public async Task<Result<WorkspaceDto>> Handle(
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
            if (WorkspacePolicy.IsActionAllowedAnonymous(workspace.Settings, action))
            {
                return workspace.ToDto();
            }
            return Result.FailureOf<WorkspaceDto>(AuthErrors.Unauthenticated);
        }

        var globalRole = userContext.GetUserRole();
        if (globalRole.IsSuccess &&
            WorkspacePolicy.IsActionAllowedGlobalRole(globalRole.Value, action))
        {
            return workspace.ToDto();
        }

        var userId = userContext.GetUserId();
        var userWorkspace = await uow.UserWorkspaceRepository
            .GetByUserAndWorkspaceIds(userId, workspace.Id);
        if (userWorkspace is null)
        {
            return Result.FailureOf<WorkspaceDto>(AuthErrors
                .Forbidden("You are not member of this workspace"));
        }

        var isAllowed = WorkspacePolicy.IsActionAllowed(userWorkspace, 
            workspace.Settings, action);
        return isAllowed
            ? workspace.ToDto()
            : Result.FailureOf<WorkspaceDto>(AuthErrors
            .Forbidden("You are not allowed to do this"));
    }
}
