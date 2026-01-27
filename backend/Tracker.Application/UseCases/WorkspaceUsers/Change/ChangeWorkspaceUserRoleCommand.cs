using MediatR;
using Tracker.Domain.Enums;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.WorkspaceUsers.Change;

public class ChangeWorkspaceUserRoleCommand : IRequest<Result>
{
    public required Guid WorkspaceId { get; set; }
    public required Guid UserId { get; set; }
    public required WorkspaceUserRole Role { get; set; }
}