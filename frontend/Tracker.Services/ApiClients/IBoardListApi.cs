using Refit;
using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.BoardList;

namespace Tracker.Services.ApiClients;

public interface IBoardListApi
{
    [Post("/api/board-lists/{id}")]
    public Task<ApiResponse<BoardListDto>> CreateBoardListAsync(Guid id, CreateBoardListRequest request);

    [Post("/api/board-lists/move")]
    public Task<ApiResponse<object>> MoveBoardListAsync(MoveBoardListRequest request);
}