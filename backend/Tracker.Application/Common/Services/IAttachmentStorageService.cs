using Tracker.Domain.Results;

namespace Tracker.Application.Common.Services;

public interface IAttachmentStorageService
{
    Task<Result<string>> GetUrlAsync(string folderName, string fileName, CancellationToken cancellationToken = default);
    Task<Result<Stream>> GetStreamAsync(string folderName, string fileName, CancellationToken cancellationToken = default);
    Task<string> UploadAsync(Stream stream, string folderName, string contentType, CancellationToken cancellationToken = default);
    Task DeleteAsync(string folderName, string fileName, CancellationToken cancellationToken = default);
}