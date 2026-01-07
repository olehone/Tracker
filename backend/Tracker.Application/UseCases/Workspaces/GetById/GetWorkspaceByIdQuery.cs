using MediatR;
using Tracker.Domain.Dtos;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Workspaces.GetById;

public class GetWorkspaceByIdQuery : IRequest<Result<WorkspaceFullDto>>
{
    public required Guid Id { get; set; }
}
