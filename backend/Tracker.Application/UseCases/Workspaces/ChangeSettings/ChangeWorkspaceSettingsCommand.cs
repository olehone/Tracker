using MediatR;
using Tracker.Domain.Enums;
using Tracker.Domain.Results;
using Tracker.Domain.ValueObjects;

namespace Tracker.Application.UseCases.Workspaces.ChangeSettings;

public class ChangeWorkspaceSettingsCommand : IRequest<Result>
{
    public required Guid WorkspaceId { get; set; }
    public required bool CanChangeSettings { get; set; }
    public required WorkspaceVisibility Visibility { get; set; }
    public required WorkspacePermissionRoles PermissionRoles { get; set; }
}