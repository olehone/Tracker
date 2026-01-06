using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.BoardItem;
using Tracker.Domain.Results;

namespace Tracker.Services.Abstraction;

public interface IBoardItemService
{
    public Task<Result<BoardItemDto>> CreateBoardItemAsync(CreateBoardItemRequest request);
    public Task<Result> MoveBoardItemAsync(MoveBoardItemRequest request);
}