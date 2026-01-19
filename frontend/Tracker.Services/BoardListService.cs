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
    public Task<Result<BoardListDto>> CreateAsync(Guid boardId, CreateBoardListRequest request)
    {
        return apiErrorHandler.ExecuteAsync(() => api.CreateAsync(boardId, request));
    }

    public Task<Result> MoveAsync(Guid id, MoveBoardListRequest request)
    {
        return apiErrorHandler.ExecuteAsync(() => api.MoveAsync(id, request));
    }

    public Task<Result> UpdateAsync(Guid id,
        UpdateBoardListRequest request)
    {
        return apiErrorHandler.ExecuteAsync(() => api.UpdateAsync(id, request));
    }

    public Task<Result> DeleteAsync(Guid id)
    {
        return apiErrorHandler.ExecuteAsync(() => api.DeleteAsync(id));
    }
}