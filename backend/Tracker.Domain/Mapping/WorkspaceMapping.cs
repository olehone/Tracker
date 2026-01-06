using Tracker.Domain.Dtos;
using Tracker.Domain.Entities;

namespace Tracker.Domain.Mapping;

public static class WorkspaceMapping
{
    public static WorkspaceSummaryDto ToSummaryDto(this Workspace workspace)
    {
        return new WorkspaceSummaryDto
        {
            Id = workspace.Id,
            Title = workspace.Title,
            Description = workspace.Description,
        };
    }

    public static WorkspaceFullDto ToFullDto(this Workspace workspace,
        WorkspacePermissionsDto permissions)
    {
        return new WorkspaceFullDto
        {
            Id = workspace.Id,
            Title = workspace.Title,
            Description = workspace.Description ?? string.Empty,
            Visibility = workspace.Visibility,
            Permissions = permissions,
            Boards = workspace.Boards
                              .Select(board => board.ToSummaryDto())
                              .ToList()
        };
    }
}