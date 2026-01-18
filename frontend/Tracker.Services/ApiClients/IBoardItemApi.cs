using Refit;
using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.BoardItem;

namespace Tracker.Services.ApiClients;

public interface IBoardItemApi
{
    [Post("/api/board-items/{id}")]
    public Task<ApiResponse<BoardItemDto>> CreateAsync(Guid id, CreateBoardItemRequest request);

    [Post("/api/board-items/move")]
    public Task<ApiResponse<object>> MoveAsync(MoveBoardItemRequest request);

    [Put("/api/board-items/{id}")]
    public Task<ApiResponse<object>> UpdateAsync(Guid id, UpdateBoardItemRequest request);

    [Delete("/api/board-items/{id}")]
    public Task<ApiResponse<object>> DeleteAsync(Guid id);
}