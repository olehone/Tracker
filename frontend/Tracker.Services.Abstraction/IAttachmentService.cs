using Tracker.Domain.Dtos;
using Tracker.Domain.Enums;
using Tracker.Domain.Results;

namespace Tracker.Services.Abstraction;

public interface IAttachmentService
{
    Task<Result<Stream>> DownloadAsync(Guid attachmentId, AttachmentType type);
    Task<Result<string>> GetUrlAsync(Guid attachmentId, AttachmentType type, bool isRedirect = false);
    Task<Result<FileDto>> UploadAsync(Guid parentId,
        Stream fileStream, string contentType, string fileName, AttachmentType type);
    Task<Result> DeleteAsync(Guid attachmentId, AttachmentType type);
}