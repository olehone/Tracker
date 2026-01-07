using Tracker.Domain.Enums;
using Tracker.Domain.ValueObjects;

namespace Tracker.API.Requests;

public class ChangeWorkspaceSettingsRequest
{
    public required bool CanChangeSettings { get; set; }
    public required WorkspaceVisibility Visibility { get; set; }
    public required WorkspacePermissionRoles PermissionRoles { get; set; }
}
