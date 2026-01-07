using Tracker.Domain.Dtos;
using Tracker.Domain.Entities;
using Tracker.Domain.Enums;
using Tracker.Domain.ValueObjects;

namespace Tracker.Domain.Mapping;

public static class WorkspaceMapping
{
    public static WorkspaceSummaryDto ToSummaryDto(this Workspace workspace)
    {
        return new WorkspaceSummaryDto
        {
            Id = workspace.Id,
            Title = workspace.Title,
            Description = workspace.Description ?? string.Empty,
        };
    }

    public static WorkspaceFullDto ToFullDto(this Workspace workspace,
        WorkspacePermissionsDto permissions,
        IReadOnlyList<Board> boards)
    {
        return new WorkspaceFullDto
        {
            Id = workspace.Id,
            Title = workspace.Title,
            Description = workspace.Description ?? string.Empty,
            Visibility = workspace.Visibility,
            PermissionRoles = workspace.PermissionRoles,
            Permissions = permissions,
            Boards = boards.Select(b => b.ToSummaryDto()).ToList()
        };
    }
}