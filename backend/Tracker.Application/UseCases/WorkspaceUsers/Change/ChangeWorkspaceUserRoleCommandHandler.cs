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

        var workspaceUser = new WorkspaceUser
        {
            UserId = request.UserId,
            WorkspaceId = request.WorkspaceId,
            Role = request.Role,
        };

        var userId = userContext.GetUserId();
        var userRole = userContext.GetUserRole();
        var workspaceRole = await uow.WorkspaceUserRepository
            .GetRoleAsync(userId, request.WorkspaceId);

        var permissions = WorkspacePolicy
            .GetPermissions(workspace.Value.PermissionRoles, workspaceRole, userRole);

        if (!WorkspacePolicy.IsActionAllowed(permissions, WorkspaceAction.ChangeWorkspace))
        {
            return AuthErrors.Forbidden();
        }

        if (request.Role == WorkspaceUserRole.Owner)
        {
            if (!WorkspacePolicy.IsActionAllowed(permissions, WorkspaceAction.ChangeOwner))
            {
                return AuthErrors.Forbidden();
            }
            await ChangeOwner(request, uow);
        }
        else
        {
            uow.WorkspaceUserRepository.Update(workspaceUser);
        }

        var sc = await uow.SaveChangesAsync(cancellationToken);

        return sc.IsFailure
            ? Error.Unknown
            : Result.Success();
    }

    private static async Task ChangeOwner(ChangeWorkspaceUserRoleCommand request, IUnitOfWork uow)
    {
        var oldOwner = await uow.WorkspaceUserRepository.GetOwnerAsync(request.WorkspaceId)!;
        var oldOwnerAsAdmin = new WorkspaceUser
        {
            UserId = oldOwner!.UserId,
            WorkspaceId = oldOwner.WorkspaceId,
            Role = WorkspaceUserRole.Admin,
        };

        var newOwner = new WorkspaceUser
        {
            UserId = request.UserId,
            WorkspaceId = request.WorkspaceId,
            Role = request.Role,
        };

        uow.WorkspaceUserRepository.Update(oldOwnerAsAdmin);
        uow.WorkspaceUserRepository.Update(newOwner);
    }
}