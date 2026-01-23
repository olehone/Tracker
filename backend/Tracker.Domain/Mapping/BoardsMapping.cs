using Tracker.Domain.Dtos;
using Tracker.Domain.Entities;

namespace Tracker.Domain.Mapping;

public static class BoardsMapping
{
    public static BoardSummaryDto ToSummaryDto(this Board board)
    {
        return new BoardSummaryDto
        {
            Id = board.Id,
            Title = board.Title,
            IsParticipating = false,
            Visibility = board.Visibility
        };
    }

    public static BoardFullDto ToFullDto(this Board board,
        BoardPermissionsDto permissions)
    {
        return new BoardFullDto
        {
            Id = board.Id,
            WorkspaceId = board.WorkspaceId,
            Title = board.Title,
            Description = board.Description ?? string.Empty,
            Visibility = board.Visibility,
            Permissions = permissions,
            PermissionRoles = board.PermissionRoles,
            BoardLists = board.BoardLists
                              .Select(boardList => boardList.ToDto())
                              .ToList(),
            BoardUsers = board.UserBoards.Select(bm =>
                new BoardUserDto
                {
                    User = bm.User.ToDto(),
                    Role = bm.Role,
                }).ToList()
        };
    }

    public static BoardListDto ToDto(this BoardList boardList)
    {
        return new BoardListDto()
        {
            Id = boardList.Id,
            Position = boardList.Position,
            Title = boardList.Title,
            Description = boardList.Description ?? string.Empty,
            BoardItems = boardList.BoardItems
                                  .Select(boardItem => boardItem.ToDto())
                                  .ToList()
        };
    }

    public static BoardItemDto ToDto(this BoardItem boardItem)
    {
        return new BoardItemDto()
        {
            Id = boardItem.Id,
            BoardListId = boardItem.BoardListId,
            Position = boardItem.Position,
            IsDone = boardItem.IsDone,
            DueDate = boardItem.DueDate,
            Importance = boardItem.Importance,
            Title = boardItem.Title,
            Description = boardItem.Description ?? string.Empty,
            Assignees = boardItem.Assignees.Select(a => a.BoardUser.UserId).ToHashSet()
        };
    }
}