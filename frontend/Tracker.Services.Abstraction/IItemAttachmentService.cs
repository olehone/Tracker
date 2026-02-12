using Tracker.Domain.Dtos;
using Tracker.Domain.Results;

namespace Tracker.Services.Abstraction;

public interface IItemAttachmentService
{
    Task<Result<Stream>> DownloadAsync(Guid attachmentId);
    Task<Result<string>> GetUrlAsync(Guid attachmentId, bool isRedirect = false);
    Task<Result<List<FileDto>>> GetAllAsync(Guid boardId, Guid itemId);
    Task<Result<FileDto>> UploadAsync(Guid boardId, Guid itemId, Stream fileStream, string contentType, string fileName);
    Task<Result> DeleteAsync(Guid attachmentId);
}