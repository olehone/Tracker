using MediatR;
using Tracker.Domain.Dtos;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.WorkspaceUsers.GetByBoardId;

public class GetUsersByWorkspaceIdQuery : IRequest<Result<List<WorkspaceUserDto>>>
{
    public required Guid WorkspaceId { get; set; }
}