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
    public Task<Result<BoardItemDto>> CreateAsync(Guid boardId, CreateBoardItemRequest request)
    {
        return apiErrorHandler.ExecuteAsync(() => api.CreateAsync(boardId, request));
    }

    public Task<Result> MoveAsync(MoveBoardItemRequest request)
    {
        return apiErrorHandler.ExecuteAsync(() => api.MoveAsync(request));
    }

    public Task<Result> UpdateAsync(Guid id, UpdateBoardItemRequest request)
    {
        return apiErrorHandler.ExecuteAsync(() => api.UpdateAsync(id, request));
    }

    public Task<Result> DeleteAsync(Guid id)
    {
        return apiErrorHandler.ExecuteAsync(() => api.DeleteAsync(id));
    }
}