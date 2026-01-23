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
    public Task<Result<BoardItemDto>> CreateAsync(Guid boardId, Guid boardListId, CreateBoardItemRequest request)
    {
        return apiErrorHandler.ExecuteAsync(() => api.CreateAsync(boardId, boardListId, request));
    }

    public Task<Result> MoveAsync(Guid boardId, Guid itemId, MoveBoardItemRequest request)
    {
        return apiErrorHandler.ExecuteAsync(() => api.MoveAsync(boardId, itemId, request));
    }

    public Task<Result> UpdateAsync(Guid boardId, Guid itemId, UpdateBoardItemRequest request)
    {
        return apiErrorHandler.ExecuteAsync(() => api.UpdateAsync(boardId, itemId, request));
    }

    public Task<Result> DeleteAsync(Guid boardId, Guid itemId)
    {
        return apiErrorHandler.ExecuteAsync(() => api.DeleteAsync(boardId, itemId));
    }

    Task<Result<BoardItemDto>> IBoardItemService.AssignAsync(Guid boardId, Guid itemId, Guid userId)
    {
        return apiErrorHandler.ExecuteAsync(() => api.AssignAsync(boardId, itemId, userId));
    }

    Task<Result<BoardItemDto>> IBoardItemService.UnassignAsync(Guid boardId, Guid itemId, Guid userId)
    {
        return apiErrorHandler.ExecuteAsync(() => api.UnassingAsync(boardId, itemId, userId));
    }
}