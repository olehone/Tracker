using Refit;
using Tracker.Domain.Dtos;
using Tracker.Domain.Enums;
using Tracker.Domain.Results;
using Tracker.Services.Abstraction;
using Tracker.Services.Abstraction.Results;
using Tracker.Services.ApiClients;

namespace Tracker.Services;

public class AttachmentService(IApiErrorHandler apiErrorHandler,
    IItemAttachmentApi api) : IAttachmentService
{
    public Task<Result<Stream>> DownloadAsync(Guid attachmentId, AttachmentType type)
    {
        return apiErrorHandler.ExecuteAsync(() => api.DownloadAsync(attachmentId, type));
    }

    public Task<Result<string>> GetUrlAsync(Guid attachmentId, AttachmentType type, bool isRedirect = false)
    {
        return apiErrorHandler.ExecuteAsync(() => api.GetUrlAsync(attachmentId, type, isRedirect));
    }

    public Task<Result<FileDto>> UploadAsync(Guid parentId,
        Stream fileStream, string contentType, string fileName, AttachmentType type)
    {
        var streamPart = new StreamPart(fileStream, fileName, contentType);
        return apiErrorHandler.ExecuteAsync(() => api.UploadAsync(parentId, streamPart, type));
    }

    public Task<Result> DeleteAsync(Guid attachmentId, AttachmentType type)
    {
        return apiErrorHandler.ExecuteAsync(() => api.DeleteAsync(attachmentId, type));
    }
}
