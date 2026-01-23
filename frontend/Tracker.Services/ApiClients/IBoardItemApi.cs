using Refit;
using Tracker.Domain.Dtos;
using Tracker.Domain.Requests;
using Tracker.Domain.Requests.BoardItem;

namespace Tracker.Services.ApiClients;

public interface IBoardItemApi
{
    [Post("/api/board/{boardId}/items/{boardListId}")]
    Task<IApiResponse<BoardItemDto>> CreateAsync(Guid boardId, Guid boardListId, CreateWithTitleRequest request);

    [Post("/api/board/{boardId}/items/move/{itemId}")]
    Task<IApiResponse> MoveAsync(Guid boardId, Guid itemId, MoveBoardItemRequest request);

    [Patch("/api/board/{boardId}/items/{itemId}")]
    Task<IApiResponse> UpdateAsync(Guid boardId, Guid itemId, UpdateBoardItemRequest request);

    [Delete("/api/board/{boardId}/items/{itemId}")]
    Task<IApiResponse> DeleteAsync(Guid boardId, Guid itemId);

    [Post("/api/board/{boardId}/items/{itemId}/assign/{userId}")]
    Task<IApiResponse<HashSet<Guid>>> AssignAsync(Guid boardId, Guid itemId, Guid userId);

    [Delete("/api/board/{boardId}/items/{itemId}/assign/{userId}")]
    Task<IApiResponse<HashSet<Guid>>> UnassingAsync(Guid boardId, Guid itemId, Guid userId);
}