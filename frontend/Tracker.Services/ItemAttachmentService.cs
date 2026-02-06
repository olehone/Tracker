using Refit;
using Tracker.Domain.Dtos;
using Tracker.Domain.Results;
using Tracker.Services.Abstraction.Results;
using Tracker.Services.ApiClients;

namespace Tracker.Services;

public class ItemAttachmentService(IApiErrorHandler apiErrorHandler, IItemAttachmentApi api) : IItemAttachmentService
{
    public Task<Result> DownloadAsync(Guid attachmentId, bool isDirect = false, bool isRedirect = false)
    {
        return apiErrorHandler.ExecuteAsync(() => api.DownloadAsync(attachmentId, isDirect, isRedirect));
    }
    public Task<Result<List<FileDto>>> GetAllAsync(Guid boardId, Guid itemId)
    {
        return apiErrorHandler.ExecuteAsync(() => api.GetAllAsync(boardId, itemId));
    }
    public Task<Result<string>> UploadAsync(Guid boardId, Guid itemId,
        Stream fileStream, string contentType, string fileName)
    {
        var streamPart = new StreamPart(fileStream, fileName, contentType);
        return apiErrorHandler.ExecuteAsync(() => api.UploadAsync(boardId, itemId, streamPart));
    }
    public Task<Result> DeleteAsync(Guid attachmentId)
    {
        return apiErrorHandler.ExecuteAsync(() => api.DeleteAsync(attachmentId));
    }
}
