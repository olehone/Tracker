using Refit;
using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.BoardItem;

namespace Tracker.Services.ApiClients;

public interface IBoardItemApi
{
    [Post("/api/board-items")]
    public Task<BoardItemDto> CreateBoardItemAsync(CreateBoardItemRequest request);
    [Post("/api/board-items/move")]
    public Task<ApiResponse<object>> MoveBoardItemAsync(MoveBoardItemRequest request);
}

