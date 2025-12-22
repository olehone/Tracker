using Tracker.Domain.Dtos;
using Tracker.Domain.Entities;

namespace Tracker.Domain.Mapping;

public static class WorkspaceMapping
{
    public static WorkspaceDto ToDto(this Workspace workspace)
    {
        return new WorkspaceDto()
        {
            Id = workspace.Id,
            Title = workspace.Title,
            Description = workspace.Description,
            Boards = workspace.Boards
                              .Select(board => board.ToSummaryDto())
                              .ToList()
        };
    }
}