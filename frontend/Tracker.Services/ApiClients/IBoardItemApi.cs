using Refit;
using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.BoardItem;
using Tracker.Domain.Requests.BoardList;

namespace Tracker.Services.ApiClients;

public interface IBoardItemApi
{
    [Post("/api/board-items/{id}")]
    public Task<ApiResponse<BoardItemDto>> CreateBoardItemAsync(Guid id, CreateBoardItemRequest request);

    [Post("/api/board-items/move")]
    public Task<ApiResponse<object>> MoveBoardItemAsync(MoveBoardItemRequest request);

    [Put("/api/board-items/{id}")]
    public Task<ApiResponse<object>> UpdateBoardItemAsync(Guid id, UpdateBoardItemRequest request);

    [Delete("/api/board-items/{id}")]
    public Task<ApiResponse<object>> DeleteBoardItemAsync(Guid id);
}