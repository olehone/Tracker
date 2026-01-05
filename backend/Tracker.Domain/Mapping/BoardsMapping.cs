using Tracker.Domain.Dtos;
using Tracker.Domain.Entities;

namespace Tracker.Domain.Mapping;

public static class BoardsMapping
{
    public static BoardSummaryDto ToSummaryDto(this Board board)
    {
        return new BoardSummaryDto()
        {
            Id = board.Id,
            Title = board.Title
        };
    }

    public static BoardFullDto ToFullDto(this Board board)
    {
        return new BoardFullDto()
        {
            Id = board.Id,
            Title = board.Title,
            Description = board.Description,
            BoardLists = board.BoardLists
                              .Select(boardList => boardList.ToDto())
                              .ToList()
        };
    }

    public static BoardListDto ToDto(this BoardList boardList)
    {
        return new BoardListDto()
        {
            Id = boardList.Id,
            Position = boardList.Position,
            Title = boardList.Title,
            Description = boardList.Description,
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
            Title = boardItem.Title,
            Description = boardItem.Description,
        };
    }
}