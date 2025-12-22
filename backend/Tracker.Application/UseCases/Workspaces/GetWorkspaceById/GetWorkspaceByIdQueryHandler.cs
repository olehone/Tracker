using MediatR;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Application.UseCases.Users.GetUserById;
using Tracker.Domain.Dtos;
using Tracker.Domain.Mapping;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Workspaces.GetWorkspaceById;

public sealed class GetWorkspaceByIdQueryHandler(
    IUnitOfWorkFactory unitOfWorkFactory)
    : IRequestHandler<GetWorkspaceByIdQuery, Result<WorkspaceDto>>
{
    public async Task<Result<WorkspaceDto>> Handle(
        GetWorkspaceByIdQuery query,
        CancellationToken cancellationToken)
    {
        await using var uow = unitOfWorkFactory.Create();

        var workspace = await uow.WorkspaceRepository.GetByIdAsync(query.Id);

        if (workspace == null)
        {
            return new Error(
                "Workspace.NotFound",
                ErrorType.NotFound,
                "Workspace with this id is not found");
        }
        return workspace.ToDto();
    }
}
