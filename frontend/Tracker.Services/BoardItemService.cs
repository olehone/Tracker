using Refit;
using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.BoardItem;
using Tracker.Services.Abstraction;
using Tracker.Services.ApiClients;

namespace Tracker.Services;

public class BoardItemService(IBoardItemApi api) : IBoardItemService
{
    public Task<BoardItemDto> CreateBoardItemAsync(CreateBoardItemRequest request)
        => api.CreateBoardItemAsync(request);

    public Task<ApiResponse<object>> MoveBoardItemAsync(MoveBoardItemRequest request)
        => api.MoveBoardItemAsync(request);
}