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
    public Task<Result<BoardItemDto>> CreateBoardItemAsync(Guid boardId, Guid boardListId, CreateBoardItemRequest request)
    {
        return apiErrorHandler.ExecuteAsync(() => api.CreateBoardItemAsync(boardId, boardListId, request));
    }

    public Task<Result> MoveBoardItemAsync(Guid boardId, Guid itemId, MoveBoardItemRequest request)
    {
        return apiErrorHandler.ExecuteAsync(() => api.MoveBoardItemAsync(boardId, itemId, request));
    }

    public Task<Result> UpdateBoardItemAsync(Guid boardId, Guid itemId, UpdateBoardItemRequest request)
    {
        return apiErrorHandler.ExecuteAsync(() => api.UpdateBoardItemAsync(boardId, itemId, request));
    }

    public Task<Result> DeleteBoardItemAsync(Guid boardId, Guid itemId)
    {
        return apiErrorHandler.ExecuteAsync(() => api.DeleteBoardItemAsync(boardId, itemId));
    }
}