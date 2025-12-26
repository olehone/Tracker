using MediatR;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Domain.Dtos;
using Tracker.Domain.Mapping;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Workspaces.GetWorkspaces;

public sealed class GetWorkspacesQueryHandler(
    IUnitOfWorkFactory unitOfWorkFactory)
    : IRequestHandler<GetWorkspacesQuery, Result<List<WorkspaceDto>>>
{
    public async Task<Result<List<WorkspaceDto>>> Handle(
        GetWorkspacesQuery request,
        CancellationToken cancellationToken)
    {
        await using var uow = unitOfWorkFactory.Create();

        var workspaces = await uow.WorkspaceRepository.GetAllWithBoardsAsync();

        return workspaces is null
            ? Error.NotFound("Workspace")
            : workspaces.Select(workspace => workspace.ToDto()).ToList();
    }
}
