using MediatR;
using Tracker.Application.Common.Auth;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Application.UseCases.Boards;
using Tracker.Application.UseCases.Workspaces;
using Tracker.Domain.Dtos;
using Tracker.Domain.Mapping;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.WorkspaceUsers.GetByBoardId;

public class GetUsersByWorkspaceIdQueryHandler(
    IUserContext userContext,
    IUnitOfWorkFactory unitOfWorkFactory)
    : IRequestHandler<GetUsersByWorkspaceIdQuery, Result<List<WorkspaceUserDto>>>
{
    public async Task<Result<List<WorkspaceUserDto>>> Handle(
        GetUsersByWorkspaceIdQuery request,
        CancellationToken cancellationToken)
    {
        await using var uow = unitOfWorkFactory.Create();

        var workspace = await WorkspaceHelper.GetWorkspaceForActionAsync(uow, userContext,
            request.WorkspaceId, WorkspaceAction.ChangeWorkspace);
        if (workspace.IsFailure)
        {
            return workspace.Error;
        }

        var workspaceUsers = await uow.UserWorkspaceRepository.GetByWorkspaceAsync(request.WorkspaceId);

        return workspaceUsers.Select(wu => new WorkspaceUserDto
        {
            User = wu.User.ToDto(),
            Role = wu.Role,
        }).ToList();
    }
}