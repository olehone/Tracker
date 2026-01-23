using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.BoardList;
using Tracker.Domain.Results;
using Tracker.Services.Abstraction;
using Tracker.Services.Abstraction.Results;
using Tracker.Services.ApiClients;

namespace Tracker.Services;

public class BoardListService(IApiErrorHandler apiErrorHandler, IBoardListApi api)
    : IBoardListService
{
    public Task<Result<BoardListDto>> CreateAsync(Guid boardId, string title)
    {
        return apiErrorHandler.ExecuteAsync(() => api.CreateAsync(boardId, request));
    }

    public Task<Result> MoveAsync(Guid boardId, Guid itemId, MoveBoardListRequest request)
    {
        return apiErrorHandler.ExecuteAsync(() => api.MoveAsync(boardId, itemId, request));
    }

    public Task<Result> UpdateAsync(Guid boardId, Guid itemId,
        UpdateBoardListRequest request)
    {
        return apiErrorHandler.ExecuteAsync(() => api.UpdateAsync(boardId, itemId, request));
    }

    public Task<Result> DeleteAsync(Guid boardId, Guid itemId)
    {
        return apiErrorHandler.ExecuteAsync(() => api.DeleteAsync(boardId, itemId));
    }
}