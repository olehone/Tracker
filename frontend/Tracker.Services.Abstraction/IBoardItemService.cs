using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.BoardItem;
using Tracker.Domain.Results;

namespace Tracker.Services.Abstraction;

public interface IBoardItemService
{
    public Task<Result<BoardItemDto>> CreateAsync(Guid boardId, CreateBoardItemRequest request);
    public Task<Result> MoveAsync(MoveBoardItemRequest request);
    public Task<Result> UpdateAsync(Guid id, UpdateBoardItemRequest request);
    public Task<Result> DeleteAsync(Guid id);
}