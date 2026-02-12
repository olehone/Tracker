using MediatR;
using Tracker.Domain.Dtos;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.ItemComments.Get;

public class LoadCommentsQuery : IRequest<Result<CursorPage<ItemCommentDto>>>
{
    public required Guid ItemId { get; set; }
    public required DateTimeOffset Before { get; set; }
    public int Take { get; set; } = 20;
}
