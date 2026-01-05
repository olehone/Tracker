using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.BoardItem;
using Tracker.Domain.Results;
using Tracker.Services.Abstraction.Entities;
using Tracker.Services.Abstraction.Results;
using Tracker.Services.ApiClients;

namespace Tracker.Services.Entities;

public class BoardItemService(IApiErrorHandler apiErrorHandler, IBoardItemApi api)
    : IBoardItemService
{
    public Task<Result<BoardItemDto>> CreateBoardItemAsync(CreateBoardItemRequest request)
        => apiErrorHandler.ExecuteAsync(request, api.CreateBoardItemAsync);

    public  Task<Result> MoveBoardItemAsync(MoveBoardItemRequest request)
        => apiErrorHandler.ExecuteAsync(request, api.MoveBoardItemAsync);
}