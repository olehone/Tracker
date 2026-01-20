using MediatR;
using Tracker.Application.Common.Auth;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Application.UseCases.Workspaces;
using Tracker.Domain.Entities;
using Tracker.Domain.Enums;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.WorkspaceUsers.Change;

public class ChangeWorkspaceUserRoleCommandHandler(IUserContext userContext,
    IUnitOfWorkFactory unitOfWorkFactory)
    : IRequestHandler<ChangeWorkspaceUserRoleCommand, Result>
{
    public async Task<Result> Handle(ChangeWorkspaceUserRoleCommand request,
        CancellationToken cancellationToken)
    {
        await using var uow = unitOfWorkFactory.Create();

        var workspace = await WorkspaceHelper.GetWorkspaceForActionAsync(uow, userContext,
            request.WorkspaceId, WorkspaceAction.ChangeWorkspace);
        if (workspace.IsFailure)
        {
            return workspace.Error;
        }

        var user = await uow.UserRepository.GetByIdAsync(request.UserId);
        if (user is null)
        {
            return Error.NotFound("User");
        }

        var workspaceUser = new UserWorkspace
        {
            UserId = request.UserId,
            WorkspaceId = request.WorkspaceId,
            Role = request.Role,
        };

        var userId = userContext.GetUserId();
        var userRole = userContext.GetUserRole();
        var workspaceRole = await uow.UserWorkspaceRepository
            .GetRoleAsync(userId, request.WorkspaceId);

        var permissions = WorkspacePolicy
            .GetPermissions(workspace.Value.PermissionRoles, workspaceRole, userRole);

        if (!WorkspacePolicy.IsActionAllowed(permissions, WorkspaceAction.ChangeWorkspace))
        {
            return AuthErrors.Forbidden();
        }

        if (request.Role == UserWorkspaceRole.Owner)
        {
            if (!WorkspacePolicy.IsActionAllowed(permissions, WorkspaceAction.ChangeOwner))
            {
                return AuthErrors.Forbidden();
            }
            await ChangeOwner(request, uow);
        }
        else
        {
            uow.UserWorkspaceRepository.Update(workspaceUser);
        }

        var sc = await uow.SaveChangesAsync(cancellationToken);

        return sc.IsFailure
            ? Error.Unknown
            : Result.Success();
    }

    private static async Task ChangeOwner(ChangeWorkspaceUserRoleCommand request, IUnitOfWork uow)
    {
        var oldOwner = await uow.UserWorkspaceRepository.GetOwnerAsync(request.WorkspaceId)!;
        var oldOwnerAsAdmin = new UserWorkspace
        {
            UserId = oldOwner!.UserId,
            WorkspaceId = oldOwner.WorkspaceId,
            Role = UserWorkspaceRole.Admin,
        };

        var newOwner = new UserWorkspace
        {
            UserId = request.UserId,
            WorkspaceId = request.WorkspaceId,
            Role = request.Role,
        };

        uow.UserWorkspaceRepository.Update(oldOwnerAsAdmin);
        uow.UserWorkspaceRepository.Update(newOwner);
    }
}