namespace Tracker.Application.Common.Services;

// Public storage with public URL for file access
public interface IAvatarStorageService
{
    Task DeleteAsync(Guid resourceId, CancellationToken cancellationToken = default);
    string GetPublicUrl(Guid resourceId);
    Task<string> UploadAsync(Stream stream, string contentType, Guid resourceId, CancellationToken cancellationToken);
}