using MediatR;
using Tracker.Domain.Enums;
using Tracker.Domain.Results;
using Tracker.Domain.ValueObjects;

namespace Tracker.Application.UseCases.Workspaces.Update;

public class UpdateWorkspaceCommand : IRequest<Result>
{
    public required Guid WorkspaceId { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required WorkspaceVisibility Visibility { get; set; }
    public required WorkspacePermissionRoles PermissionRoles { get; set; }
}