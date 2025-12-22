using MediatR;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Domain.Dtos;
using Tracker.Domain.Results;
using Tracker.Domain.Mapping;

namespace Tracker.Application.UseCases.Workspaces.GetWorkspaces;

public sealed class GetWorkspacesQueryHandler(
    IUnitOfWorkFactory unitOfWorkFactory)
    : IRequestHandler<GetWorkspacesQuery, Result<List<WorkspaceDto>>>
{
    public async Task<Result<List<WorkspaceDto>>> Handle(
        GetWorkspacesQuery query,
        CancellationToken cancellationToken)
    {
        await using var uow = unitOfWorkFactory.Create();

        var workspaces = await uow.WorkspaceRepository.GetAllAsync();

        if (workspaces == null)
        {
            return new Error(
                "Workspace.NotFound",
                ErrorType.NotFound,
                "There are no workspaces yet");
        }

        return workspaces.Select(workspace => workspace.ToDto()).ToList();
    }
}
