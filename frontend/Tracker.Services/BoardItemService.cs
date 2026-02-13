using Tracker.Domain.Dtos;
using Tracker.Domain.Requests;
using Tracker.Domain.Requests.BoardItem;
using Tracker.Domain.Results;
using Tracker.Services.Abstraction;
using Tracker.Services.Abstraction.Results;
using Tracker.Services.ApiClients;

namespace Tracker.Services;

public class BoardItemService(IApiErrorHandler apiErrorHandler, IBoardItemApi api)
    : IBoardItemService
{
    public Task<Result<BoardItemDto>> CreateAsync(Guid boardId, Guid boardListId, string title)
    {
        var request = new CreateWithTitleRequest { Title = title };
        return apiErrorHandler.ExecuteAsync(() => api.CreateAsync(boardId, boardListId, request));
    }

    public Task<Result> MoveAsync(Guid boardId, Guid itemId, MoveBoardItemRequest request)
    {
        return apiErrorHandler.ExecuteAsync(() => api.MoveAsync(boardId, itemId, request));
    }

    public Task<Result<BoardItemDto>> UpdateAsync(Guid boardId, Guid itemId, UpdateBoardItemRequest request)
    {
        return apiErrorHandler.ExecuteAsync(() => api.UpdateAsync(boardId, itemId, request));
    }

    public Task<Result> DeleteAsync(Guid boardId, Guid itemId)
    {
        return apiErrorHandler.ExecuteAsync(() => api.DeleteAsync(boardId, itemId));
    }

    Task<Result<HashSet<Guid>>> IBoardItemService.AssignAsync(Guid boardId, Guid itemId, Guid userId)
    {
        return apiErrorHandler.ExecuteAsync(() => api.AssignAsync(boardId, itemId, userId));
    }

    Task<Result<HashSet<Guid>>> IBoardItemService.UnassignAsync(Guid boardId, Guid itemId, Guid userId)
    {
        return apiErrorHandler.ExecuteAsync(() => api.UnassingAsync(boardId, itemId, userId));
    }

    public Task<Result<List<FileDto>>> GetAttachmentsAsync(Guid boardId, Guid itemId)
    {
        return apiErrorHandler.ExecuteAsync(() => api.GetAttachmentsAsync(boardId, itemId));
    }
}