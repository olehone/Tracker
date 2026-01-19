using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.BoardItem;
using Tracker.Domain.Results;

namespace Tracker.Services.Abstraction;

public interface IBoardItemService
{
    Task<Result<BoardItemDto>> CreateAsync(Guid boardId, Guid boardListId, CreateBoardItemRequest request);
    Task<Result> MoveAsync(Guid boardId, Guid itemId, MoveBoardItemRequest request);
    Task<Result> UpdateAsync(Guid boardId, Guid itemId, UpdateBoardItemRequest request);
    Task<Result> DeleteAsync(Guid boardId, Guid itemId);
}