using Refit;
using Tracker.Domain.Dtos;
using Tracker.Domain.Enums;

namespace Tracker.Services.ApiClients;

public interface IAttachmentApi
{
    [Get("/api/attachments/{attachmentId}")]
    Task<IApiResponse<Stream>> DownloadAsync(Guid attachmentId, [Query] AttachmentType type);

    [Get("/api/attachments/{attachmentId}/url")]
    Task<IApiResponse<string>> GetUrlAsync(Guid attachmentId, [Query] AttachmentType type, [Query] bool isRedirect);

    [Multipart]
    [Post("/api/attachments/{parentId}")]
    Task<IApiResponse<FileDto>> UploadAsync(Guid parentId, [AliasAs("File")] StreamPart file, [Query] AttachmentType type);

    [Delete("/api/attachments/{attachmentId}")]
    Task<IApiResponse> DeleteAsync(Guid attachmentId, [Query] AttachmentType type);
}
