using MediatR;
using Tracker.Application.Common.Auth;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Application.UseCases.Boards;
using Tracker.Application.UseCases.BoardUsers.Change;
using Tracker.Application.UseCases.Workspaces;
using Tracker.Domain.Entities;
using Tracker.Domain.Enums;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.WorkspaceUsers.Remove;

public class RemoveUserFromWorkspaceCommandHandler(
    IUserContext userContext,
    IUnitOfWorkFactory unitOfWorkFactory)
    : IRequestHandler<RemoveUserFromWorkspaceCommand, Result>
{
    public async Task<Result> Handle(
        RemoveUserFromWorkspaceCommand request,
        CancellationToken cancellationToken)
    {
        await using var uow = unitOfWorkFactory.Create();

        var workspace = await WorkspaceHelper.GetWorkspaceForActionAsync(uow, userContext,
            request.WorkspaceId, WorkspaceAction.ChangeWorkspace);
        if (workspace.IsFailure)
        {
            return workspace.Error;
        }

        var userWorkspace = await uow.UserWorkspaceRepository
            .GetByUserAndWorkspaceAsync(request.UserId, request.WorkspaceId);
        if (userWorkspace is null)
        {
            return Error.NotFound("User", "workspace");
        }

        var userId = userContext.GetUserId();

        if (userWorkspace.Role == UserWorkspaceRole.Owner && userWorkspace.UserId == userId)
        {
            await uow.WorkspaceRepository.RemoveAsync(workspace.Value.Id);
        }
        else
        {
            await uow.UserWorkspaceRepository.RemoveAsync(userWorkspace.Id);
        }

        var sc = await uow.SaveChangesAsync(cancellationToken);

        return sc.IsFailure
            ? Error.Unknown
            : Result.Success();
    }

}