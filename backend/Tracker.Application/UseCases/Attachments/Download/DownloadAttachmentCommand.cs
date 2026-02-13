using MediatR;
using Tracker.Domain.Enums;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Attachments.Download;

public class DownloadAttachmentCommand : IRequest<Result<AttachmentResponse>>
{
    public required Guid AttachmentId { get; set; }
    public required AttachmentType Type { get; set; }
    public required bool ForceDirect { get; set; }
}
