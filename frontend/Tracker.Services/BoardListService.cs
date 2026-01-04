using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.BoardList;
using Tracker.Domain.Results;
using Tracker.Services.Abstraction.Entities;
using Tracker.Services.Abstraction.Results;
using Tracker.Services.ApiClients;

namespace Tracker.Services.Entities;

public class BoardListService(IApiErrorHandler apiErrorHandler, IBoardListApi api)
    : IBoardListService
{
    public Task<Result<BoardListDto>> CreateBoardListAsync(CreateBoardListRequest request)
    {
        return apiErrorHandler.ExecuteAsync(request, api.CreateBoardListAsync);
    }

    public Task<Result> MoveBoardListAsync(MoveBoardListRequest request)
    {
        return apiErrorHandler.ExecuteAsync(request, api.MoveBoardListAsync);
    }
}