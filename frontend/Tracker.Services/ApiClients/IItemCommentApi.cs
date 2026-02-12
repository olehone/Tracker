using Refit;
using Tracker.Domain.Dtos;
using Tracker.Domain.Requests;
using Tracker.Domain.Requests.ItemComment;

namespace Tracker.Services.ApiClients;

public interface IItemCommentApi
{
    [Get("/api/items/{itemId}/comments")]
    Task<IApiResponse<CursorPage<ItemCommentDto>>> GetAsync(Guid itemId, CursorTimeRequest request);

    [Post("/api/items/{itemId}/comments")]
    Task<IApiResponse<ItemCommentDto>> CreateAsync(Guid itemId, CreateCommentRequest request);
}