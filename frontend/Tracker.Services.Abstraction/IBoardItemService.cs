using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.BoardItem;
using Tracker.Domain.Results;

namespace Tracker.Services.Abstraction;

public interface IBoardItemService
{
    Task<Result<BoardItemDto>> CreateAsync(Guid boardId, Guid boardListId, string title);
    Task<Result> MoveAsync(Guid boardId, Guid itemId, MoveBoardItemRequest request);
    Task<Result<BoardItemDto>> UpdateAsync(Guid boardId, Guid itemId, UpdateBoardItemRequest request);
    Task<Result> DeleteAsync(Guid boardId, Guid itemId);
    Task<Result<HashSet<Guid>>> AssignAsync(Guid boardId, Guid itemId, Guid userId);
    Task<Result<HashSet<Guid>>> UnassignAsync(Guid boardId, Guid itemId, Guid userId);

}