using MediatR;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.ItemComments.Delete;

public class DeleteItemCommentCommand : IRequest<Result>
{
    public required Guid CommentId { get; set; }
}