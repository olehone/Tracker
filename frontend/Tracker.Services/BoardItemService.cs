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
    public Task<Result<BoardItemDto>> CreateBoardItemAsync(Guid boardId, CreateBoardItemRequest request)
    {
        return apiErrorHandler.ExecuteAsync(() => api.CreateBoardItemAsync(boardId, request));
    }

    public Task<Result> MoveBoardItemAsync(MoveBoardItemRequest request)
    {
        return apiErrorHandler.ExecuteAsync(() => api.MoveBoardItemAsync(request));
    }

    public Task<Result> UpdateBoardItemAsync(Guid id, UpdateBoardItemRequest request)
    {
        return apiErrorHandler.ExecuteAsync(() => api.UpdateBoardItemAsync(id, request));
    }

    public Task<Result> DeleteBoardItemAsync(Guid id)
    {
        return apiErrorHandler.ExecuteAsync(() => api.DeleteBoardItemAsync(id));
    }
}