using Refit;
using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.ItemComment;

namespace Tracker.Services.ApiClients;

public interface IItemCommentApi
{
    [Post("/api/items/{itemId}/comments")]
    Task<IApiResponse<ItemCommentDto>> CreateAsync(Guid itemId, CreateCommentRequest request);
}