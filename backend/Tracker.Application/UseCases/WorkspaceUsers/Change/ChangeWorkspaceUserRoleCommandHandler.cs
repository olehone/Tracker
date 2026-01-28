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

        var action = request.Role == WorkspaceUserRole.Owner
            ? WorkspaceAction.ChangeOwner
            : WorkspaceAction.ChangeBoard;

        var workspaceResult = await WorkspaceHelper.GetWorkspaceForActionAsync(uow, userContext,
            request.WorkspaceId, action);
        if (workspaceResult.IsFailure)
        {
            return workspaceResult.Error;
        }

        var user = await uow.UserRepository.GetByIdAsync(request.UserId);
        if (user is null)
        {
            return Error.NotFound("User");
        }

        var workspaceUser = await uow.WorkspaceUserRepository.GetAsync(request.UserId, request.WorkspaceId);
        if (workspaceUser is null)
        {
            return WorkspaceErrors.UserNotInWorkspace;
        }
        workspaceUser.Role = request.Role;

        if (action == WorkspaceAction.ChangeOwner)
        {
            var oldOwner = await uow.WorkspaceUserRepository.GetOwnerAsync(workspaceUser.WorkspaceId);
            oldOwner!.Role = WorkspaceUserRole.Admin;

            uow.WorkspaceUserRepository.Update(oldOwner);
        }

        uow.WorkspaceUserRepository.Update(workspaceUser);

        var sc = await uow.SaveChangesAsync(cancellationToken);

        return sc.IsFailure
            ? Error.Unknown
            : Result.Success();
    }
}