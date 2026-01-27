using MediatR;
using Tracker.Application.Common.Auth;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Application.UseCases.Workspaces;
using Tracker.Domain.Dtos;
using Tracker.Domain.Entities;
using Tracker.Domain.Mapping;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.WorkspaceUsers.Add;

public class AddUserToWorkspaceCommandHandler(
    IUserContext userContext,
    IUnitOfWorkFactory unitOfWorkFactory)
    : IRequestHandler<AddUserToWorkspaceCommand, Result<WorkspaceUserDto>>
{
    public async Task<Result<WorkspaceUserDto>> Handle(
        AddUserToWorkspaceCommand request,
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

        var userBoard = await uow.WorkspaceUserRepository
            .GetAsync(request.UserId, request.WorkspaceId);
        if (userBoard is not null)
        {
            return Error.AlreadyExists("User", "Workspace", user.Username);
        }

        var workspaceUser = new WorkspaceUser
        {
            UserId = request.UserId,
            WorkspaceId = request.WorkspaceId,
            Role = request.Role,
        };

        await uow.WorkspaceUserRepository.AddAsync(workspaceUser);

        var sc = await uow.SaveChangesAsync(cancellationToken);
        var dto = new WorkspaceUserDto
        {
            User = user.ToDto(),
            Role = request.Role
        };

        return sc.IsFailure
            ? Error.Unknown
            : dto;
    }
}