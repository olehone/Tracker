using Refit;
using Tracker.Domain.Dtos;

namespace Tracker.Services.ApiClients;

public interface IItemAttachmentApi
{
    [Get("/attachments/{attachmentId}")]
    Task<IApiResponse<string>> DownloadAsync(Guid attachmentId, [Query] bool isDirect, [Query] bool isRedirect);

    [Get("/api/board/{boardId}/items/{itemId}/attachments")]
    Task<IApiResponse<List<FileDto>>> GetAllAsync(Guid boardId, Guid itemId);

    [Multipart]
    [Post("/api/board/{boardId}/items/{itemId}/attachments")]
    Task<IApiResponse<FileDto>> UploadAsync(Guid boardId, Guid itemId, [AliasAs("File")] StreamPart file);

    [Delete("/attachments/{attachmentId}")]
    Task<IApiResponse> DeleteAsync(Guid attachmentId);
}
