using Refit;
using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.BoardList;
using Tracker.Services.Abstraction;
using Tracker.Services.ApiClients;

namespace Tracker.Services;

public class BoardListService(IBoardListApi api) : IBoardListService
{
    public Task<BoardListDto> CreateBoardListAsync(CreateBoardListRequest request)
        => api.CreateBoardListAsync(request);

    public Task<ApiResponse<object>> MoveBoardListAsync(MoveBoardListRequest request)
        => api.MoveBoardListAsync(request);
}