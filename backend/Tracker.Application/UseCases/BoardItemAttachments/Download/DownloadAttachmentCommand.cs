using MediatR;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.BoardItemAttachments.Download;

public class DownloadAttachmentCommand : IRequest<Result<AttachmentResponse>>
{
    public required Guid AttachmentId { get; set; }
    public required bool ForceDirect { get; set; }
}
