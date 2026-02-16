using MediatR;
using Tracker.Domain.Dtos;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.ItemComments.Update;

public class UpdateItemCommentCommand : IRequest<Result<ItemCommentDto>>
{
    public required Guid CommentId { get; set; }
    public required string Content { get; set; }
}