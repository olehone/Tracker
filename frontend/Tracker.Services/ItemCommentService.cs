using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.ItemComment;
using Tracker.Domain.Results;
using Tracker.Services.Abstraction;
using Tracker.Services.Abstraction.Results;
using Tracker.Services.ApiClients;

namespace Tracker.Services;

public class ItemCommentService(IApiErrorHandler apiErrorHandler,
    IItemCommentApi api) : IItemCommentService

{
    public Task<Result<ItemCommentDto>> CreateAsync(Guid itemId, CreateCommentRequest request)
    {
        return apiErrorHandler.ExecuteAsync(() => api.CreateAsync(itemId, request));
    }
}