using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.BoardItem;
using Tracker.Domain.Results;

namespace Tracker.Services.Abstraction;

public interface IBoardItemService
{
    public Task<Result<BoardItemDto>> CreateBoardItemAsync(Guid boardId, Guid boardListId, CreateBoardItemRequest request);
    public Task<Result> MoveBoardItemAsync(Guid boardId, Guid itemId, MoveBoardItemRequest request);
    public Task<Result> UpdateBoardItemAsync(Guid boardId, Guid itemId, UpdateBoardItemRequest request);
    public Task<Result> DeleteBoardItemAsync(Guid boardId, Guid itemId);
}