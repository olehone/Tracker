using MediatR;
using Tracker.Domain.Dtos;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Workspaces.GetWorkspaceById;

public class GetWorkspaceByIdQuery : IRequest<Result<WorkspaceDto>>
{
    public Guid Id { get; set; }
}
