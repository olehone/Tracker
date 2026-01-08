using Tracker.Domain.Enums;
using Tracker.Domain.ValueObjects;

namespace Tracker.Domain.Dtos;

public class WorkspaceFullDto
{
    public required Guid Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required WorkspaceVisibility Visibility { get; set; }
    public required WorkspacePermissionRoles PermissionRoles { get; set; }
    public required WorkspacePermissionsDto Permissions { get; set; }
    public required List<BoardSummaryDto> Boards { get; set; }
}