using MediatR;
using Tracker.Domain.Dtos;
using Tracker.Domain.Enums;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.WorkspaceUsers.Add;

public class AddUserToWorkspaceCommand : IRequest<Result<WorkspaceUserDto>>
{
    public required Guid WorkspaceId { get; set; }
    public required Guid UserId { get; set; }
    public required UserWorkspaceRole Role { get; set; }
}