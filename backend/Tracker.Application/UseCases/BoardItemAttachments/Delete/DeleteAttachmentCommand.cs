using MediatR;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.BoardItemAttachments.Delete;

public class DeleteAttachmentCommand: IRequest<Result>
{
    public required Guid BoardId { get; set; }
    public required Guid BoardItemId { get; set; }
    public required Guid AttachmentId { get; set; }
}
