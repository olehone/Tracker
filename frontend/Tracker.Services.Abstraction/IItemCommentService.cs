using Tracker.Domain.Dtos;
using Tracker.Domain.Requests;
using Tracker.Domain.Requests.ItemComment;
using Tracker.Domain.Results;

namespace Tracker.Services.Abstraction;

public interface IItemCommentService
{
    Task<Result<CursorPage<ItemCommentDto>>> GetAsync(Guid itemId, CursorTimeRequest request);
    Task<Result<ItemCommentDto>> CreateAsync(Guid itemId, CreateCommentRequest request);
}