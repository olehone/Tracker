using MediatR;
using Tracker.Application.Common.Auth;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Domain.Dtos;
using Tracker.Domain.Entities;
using Tracker.Domain.Enums;
using Tracker.Domain.Mapping;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Workspaces.Create;

public sealed class CreateWorkspaceCommandHandler(
    IUnitOfWorkFactory unitOfWorkFactory,
    IUserContext userContext)
    : IRequestHandler<CreateWorkspaceCommand, Result<WorkspaceFullDto>>
{
    public async Task<Result<WorkspaceFullDto>> Handle(
        CreateWorkspaceCommand request,
        CancellationToken cancellationToken)
    {
        if (!userContext.IsAuthenticated())
        {
            return AuthErrors.Unauthenticated;
        }
        var userId = userContext.GetUserId();

        await using var uow = unitOfWorkFactory.Create();
        var workspace = new Workspace
        {
            Title = request.Title,
            Description = request.Description,
        };

        var userWorkspace = new UserWorkspace
        {
            UserId = userId,
            WorkspaceId = workspace.Id,
            Role = UserWorkspaceRole.Owner
        };

        await uow.WorkspaceRepository.AddAsync(workspace);
        await uow.UserWorkspaceRepository.AddAsync(userWorkspace);

        var sc = await uow.SaveChangesAsync(cancellationToken);

        var permissions = WorkspacePolicy
            .GetPermissions(workspace.PermissionRoles, userWorkspace.Role);

        return sc.IsFailure
            ? Error.Unknown
            : workspace.ToFullDto(permissions);
    }
}
