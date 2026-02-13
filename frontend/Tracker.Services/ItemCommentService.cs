using System.IO;
using Refit;
using Tracker.Domain.Dtos;
using Tracker.Domain.Requests;
using Tracker.Domain.Requests.ItemComment;
using Tracker.Domain.Results;
using Tracker.Services.Abstraction;
using Tracker.Services.Abstraction.Results;
using Tracker.Services.ApiClients;

namespace Tracker.Services;

public class ItemCommentService(IApiErrorHandler apiErrorHandler,
    IItemCommentApi api) : IItemCommentService

{
    public Task<Result<CursorPage<ItemCommentDto>>> GetAsync(Guid itemId, CursorTimeRequest request)
    {
        return apiErrorHandler.ExecuteAsync(() => api.GetAsync(itemId, request));
    }

    public Task<Result<ItemCommentDto>> CreateAsync(Guid itemId, CreateCommentRequest request)
    {
        return apiErrorHandler.ExecuteAsync(() => api.CreateAsync(itemId, request));
    }

    public Task<Result<FileDto>> UploadAttachmentAsync(Guid commentId,
        Stream fileStream, string contentType, string fileName)
    {
        var streamPart = new StreamPart(fileStream, fileName, contentType);
        return apiErrorHandler.ExecuteAsync(() => api.UploadAttachmentAsync(commentId, streamPart));
    }
}