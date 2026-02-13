using MediatR;
using Tracker.Domain.Dtos;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.ItemComments.UploadAttachment;

public class UploadCommentAttachmentCommand : IRequest<Result<FileDto>>
{
    public required Guid CommentId { get; set; }
    public required Stream Content { get; set; }
    public required string ContentType { get; set; }
    public required string FileName { get; set; }
    public required long ContentLength { get; set; }
}
