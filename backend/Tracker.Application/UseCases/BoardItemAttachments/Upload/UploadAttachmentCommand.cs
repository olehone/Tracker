using MediatR;
using Tracker.Domain.Dtos;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.BoardItemAttachments.Upload;

public class UploadAttachmentCommand: IRequest<Result<FileDto>>
{
    public required Guid BoardId { get; set; }
    public required Guid BoardItemId { get; set; }
    public required Stream Content { get; set; }
    public required string ContentType { get; set; }
    public required string FileName { get; set; }
    public required long ContentLength { get; set; }
}
