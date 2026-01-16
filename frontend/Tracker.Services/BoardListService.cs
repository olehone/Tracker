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
    public Task<Result<BoardListDto>> CreateBoardListAsync(Guid boardId, CreateBoardListRequest request)
    {
        return apiErrorHandler.ExecuteAsync(() => api.CreateBoardListAsync(boardId, request));
    }

    public Task<Result> MoveBoardListAsync(Guid id, MoveBoardListRequest request)
    {
        return apiErrorHandler.ExecuteAsync(() => api.MoveBoardListAsync(id, request));
    }

    public Task<Result<BoardListDto>> UpdateBoardListAsync(Guid id,
        UpdateBoardListRequest request)
    {
        return apiErrorHandler.ExecuteAsync(() => api.UpdateBoardListAsync(id, request));
    }

    public Task<Result<BoardListDto>> DeleteBoardListAsync(Guid id)
    {
        return apiErrorHandler.ExecuteAsync(() => api.DeleteBoardListAsync(id));
    }
}