using Refit;
using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.BoardList;

namespace Tracker.Services.ApiClients;

public interface IBoardListApi
{
    [Post("/api/board/{boardId}/lists")]
    public Task<ApiResponse<BoardListDto>> CreateAsync(Guid boardId, CreateBoardListRequest request);

    [Post("/api/board/{boardId}/lists/{listId}/move")]
    public Task<ApiResponse<object>> MoveAsync(Guid boardId, Guid listId, MoveBoardListRequest request);

    [Put("/api/board/{boardId}/lists/{listId}")]
    public Task<ApiResponse<object>> UpdateAsync(Guid boardId, Guid listId, UpdateBoardListRequest request);

    [Delete("/api/board/{boardId}/lists/{listId}")]
    public Task<ApiResponse<object>> DeleteAsync(Guid boardId, Guid listId);
}