using MediatR;
using Tracker.Application.Common.Auth;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Domain.Entities;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Workspaces.Update;

public class UpdateWorkspaceCommandHandler(
    IUserContext userContext,
    IUnitOfWorkFactory unitOfWorkFactory)
    : IRequestHandler<UpdateWorkspaceCommand, Result>
{
    public async Task<Result> Handle(UpdateWorkspaceCommand request,
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
        var newWorkspace = new Workspace
        {
            Id = request.WorkspaceId,
            Title = request.Title,
            Description = request.Description,
            Visibility = request.Visibility,
            PermissionRoles = request.PermissionRoles,
        };

        uow.WorkspaceRepository.Update(newWorkspace);
        var result = await uow.SaveChangesAsync(cancellationToken);
        if (result.IsFailure)
        {
            return result;
        }
        return Result.Success();
    }
}