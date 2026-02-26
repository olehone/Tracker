using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.Board;
using Tracker.Domain.Results;
using Tracker.Services.Abstraction.Board;
using Tracker.Services.Abstraction.Results;
using Tracker.Services.ApiClients;

namespace Tracker.Services.Board;

public class BoardService(IApiErrorHandler apiErrorHandler, IBoardsApi api) : IBoardService
{
    public Task<Result<BoardFullDto>> GetByIdAsync(Guid id)
    {
        return apiErrorHandler.ExecuteAsync(() => api.GetByIdAsync(id));
    }

    public Task<Result> UpdateAsync(Guid id, UpdateBoardRequest request)
    {
        return apiErrorHandler.ExecuteAsync(() => api.UpdateAsync(id, request));
    }

    public Task<Result> DeleteAsync(Guid id)
    {
        return apiErrorHandler.ExecuteAsync(() => api.DeleteAsync(id));
    }

    public Task<Result<List<BoardSummaryDto>>> GetForCurrentUserAsync()
    {
        return apiErrorHandler.ExecuteAsync(api.GetForCurrentUserAsync);
    }

    public Task<Result<Guid>> StartCallAsync(Guid id)
    {
        return apiErrorHandler.ExecuteAsync(() => api.StartCallAsync(id));
    }

    public Task<Result> ArchiveAsync(Guid id)
    {
        return apiErrorHandler.ExecuteAsync(() => api.ArchiveAsync(id));
    }

    public Task<Result> UnarchiveAsync(Guid id)
    {
        return apiErrorHandler.ExecuteAsync(() => api.UnarchiveAsync(id));
    }
}