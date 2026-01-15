using Refit;
using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.BoardList;

namespace Tracker.Services.ApiClients;

public interface IBoardListApi
{
    [Post("/api/board-lists/{boardId}")]
    public Task<ApiResponse<BoardListDto>> CreateBoardListAsync(Guid boardId, CreateBoardListRequest request);

    [Post("/api/board-lists/{id}/move")]
    public Task<ApiResponse<object>> MoveBoardListAsync(Guid id, MoveBoardListRequest request);

    [Put("/api/board-lists/{id}")]
    public Task<ApiResponse<BoardListDto>> UpdateBoardListAsync(Guid id, UpdateBoardListRequest request);

    [Delete("/api/board-lists/{id}")]
    public Task<ApiResponse<BoardListDto>> DeleteBoardListAsync(Guid id);
}