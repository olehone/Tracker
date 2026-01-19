using Refit;
using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.BoardList;

namespace Tracker.Services.ApiClients;

public interface IBoardListApi
{
    [Post("/api/board-lists/{boardId}")]
    public Task<ApiResponse<BoardListDto>> CreateAsync(Guid boardId, CreateBoardListRequest request);

    [Post("/api/board-lists/{id}/move")]
    public Task<ApiResponse<object>> MoveAsync(Guid id, MoveBoardListRequest request);

    [Put("/api/board-lists/{id}")]
    public Task<ApiResponse<object>> UpdateAsync(Guid id, UpdateBoardListRequest request);

    [Delete("/api/board-lists/{id}")]
    public Task<ApiResponse<object>> DeleteAsync(Guid id);
}