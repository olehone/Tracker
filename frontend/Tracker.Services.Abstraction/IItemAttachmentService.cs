using Tracker.Domain.Dtos;
using Tracker.Domain.Results;

namespace Tracker.Services;

public interface IItemAttachmentService
{
    Task<Result> DeleteAsync(Guid attachmentId);
    Task<Result<string>> DownloadAsync(Guid attachmentId, bool isDirect = false, bool isRedirect = false);
    Task<Result<List<FileDto>>> GetAllAsync(Guid boardId, Guid itemId);
    Task<Result<string>> UploadAsync(Guid boardId, Guid itemId, Stream fileStream, string contentType, string fileName);
}