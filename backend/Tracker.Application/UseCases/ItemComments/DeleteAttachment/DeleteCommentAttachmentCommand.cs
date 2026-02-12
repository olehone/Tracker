using MediatR;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.ItemComments.Delete;

public class DeleteCommentAttachmentCommand : IRequest<Result>
{
    public required Guid AttachmentId { get; set; }
}
