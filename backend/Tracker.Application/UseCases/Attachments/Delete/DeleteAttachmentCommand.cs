using MediatR;
using Tracker.Domain.Enums;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Attachments.Delete;

public class DeleteAttachmentCommand : IRequest<Result>
{
    public required Guid AttachmentId { get; set; }
    public required AttachmentType Type { get; set; }
}
