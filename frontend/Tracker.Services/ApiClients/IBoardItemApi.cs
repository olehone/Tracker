using Refit;
using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.BoardItem;

namespace Tracker.Services.ApiClients;

public interface IBoardItemApi
{
    [Post("/api/board/{boardId}/items/{boardListId}")]
    Task<ApiResponse<BoardItemDto>> CreateAsync(Guid boardId, Guid boardListId, CreateBoardItemRequest request);

    [Post("/api/board/{boardId}/items/move/{itemId}")]
    Task<ApiResponse<object>> MoveAsync(Guid boardId, Guid itemId, MoveBoardItemRequest request);

    [Put("/api/board/{boardId}/items/{itemId}")]
    Task<ApiResponse<object>> UpdateAsync(Guid boardId, Guid itemId, UpdateBoardItemRequest request);

    [Delete("/api/board/{boardId}/items/{itemId}")]
    Task<ApiResponse<object>> DeleteAsync(Guid boardId, Guid itemId);
}