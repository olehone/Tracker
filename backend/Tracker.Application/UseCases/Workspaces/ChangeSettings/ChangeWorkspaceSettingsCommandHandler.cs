using MediatR;
using Tracker.Application.Common.Auth;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Workspaces.ChangeSettings;

public class ChangeWorkspaceSettingsCommandHandler(
    IUserContext userContext,
    IUnitOfWorkFactory unitOfWorkFactory)
    : IRequestHandler<ChangeWorkspaceSettingsCommand, Result>
{
    public async Task<Result> Handle(ChangeWorkspaceSettingsCommand request,
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

        var result = await uow.WorkspaceRepository
            .ChangeVisibility(request.WorkspaceId, request.Visibility);
        if (result.IsFailure)
        {
            return result;
        }

        result = await uow.WorkspaceRepository
            .ChangePermissionRoles(request.WorkspaceId, request.PermissionRoles);

        if (result.IsFailure)
        {
            return result;
        }
        return Result.Success();
    }
}