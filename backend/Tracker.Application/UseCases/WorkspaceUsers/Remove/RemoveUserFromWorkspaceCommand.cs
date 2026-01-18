using MediatR;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.WorkspaceUsers.Remove;

public class RemoveUserFromWorkspaceCommand : IRequest<Result>
{
    public required Guid WorkspaceId { get; set; }
    public required Guid UserId { get; set; }
}