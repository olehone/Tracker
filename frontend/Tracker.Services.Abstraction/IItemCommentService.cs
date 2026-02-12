using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.ItemComment;
using Tracker.Domain.Results;

namespace Tracker.Services.Abstraction;

public interface IItemCommentService
{
    Task<Result<ItemCommentDto>> CreateAsync(Guid itemId, CreateCommentRequest request);
}