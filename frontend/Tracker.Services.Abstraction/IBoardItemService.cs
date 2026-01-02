using Refit;
using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.BoardItem;

namespace Tracker.Services.Abstraction;

public interface IBoardItemService
{
    public Task<BoardItemDto> CreateBoardItemAsync(CreateBoardItemRequest request);
    public Task<ApiResponse<object>> MoveBoardItemAsync(MoveBoardItemRequest request);
}

