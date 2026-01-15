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
    public Task<Result<BoardListDto>> CreateBoardListAsync(CreateBoardListRequest request)
    {
        return apiErrorHandler.ExecuteAsync(() => api.CreateBoardListAsync(request));
    }

    public Task<Result> MoveBoardListAsync(MoveBoardListRequest request)
    {
        return apiErrorHandler.ExecuteAsync(() => api.MoveBoardListAsync(request));
    }
}