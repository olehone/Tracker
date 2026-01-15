using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.BoardItem;
using Tracker.Domain.Results;
using Tracker.Services.Abstraction;
using Tracker.Services.Abstraction.Results;
using Tracker.Services.ApiClients;

namespace Tracker.Services;

public class BoardItemService(IApiErrorHandler apiErrorHandler, IBoardItemApi api)
    : IBoardItemService
{
    public Task<Result<BoardItemDto>> CreateBoardItemAsync(CreateBoardItemRequest request)
    {
        return apiErrorHandler.ExecuteAsync(() => api.CreateBoardItemAsync(request));
    }

    public Task<Result> MoveBoardItemAsync(MoveBoardItemRequest request)
    {
        return apiErrorHandler.ExecuteAsync(() => api.MoveBoardItemAsync(request));
    }
}