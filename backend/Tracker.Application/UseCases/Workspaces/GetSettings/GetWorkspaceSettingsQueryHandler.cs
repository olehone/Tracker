using MediatR;
using Tracker.Application.Common.Auth;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Domain.Dtos;
using Tracker.Domain.Mapping;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Workspaces.GetSettings;

public class GetWorkspaceSettingsQueryHandler(
    IUserContext userContext,
    IUnitOfWorkFactory unitOfWorkFactory)
    : IRequestHandler<GetWorkspaceSettingsQuery, Result<WorkspaceSettingsDto>>
{
    public async Task<Result<WorkspaceSettingsDto>> Handle(
        GetWorkspaceSettingsQuery request,
        CancellationToken cancellationToken)
    {
        if (!userContext.IsAuthenticated())
        {
            return AuthErrors.Unauthenticated;
        }

        await using var uow = unitOfWorkFactory.Create();
        var workspace = await uow.WorkspaceRepository
            .GetByIdAsync(request.WorkspaceId);
        if (workspace is null)
        {
            return Error.NotFound("Workspace");
        }

        var userId = userContext.GetUserId();
        var userRole = userContext.GetUserRole();
        var workspaceRole = await uow.UserWorkspaceRepository
            .GetRole(userId, request.WorkspaceId);

        var canChange = WorkspacePolicy.CanChangeSettings(userRole, workspaceRole);
        if (!canChange)
        {
            return AuthErrors.Forbidden();
        }

        return workspace.ToSettingsDto(canChange);
    }
}