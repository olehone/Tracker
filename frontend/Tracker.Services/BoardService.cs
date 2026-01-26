using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.Board;
using Tracker.Domain.Results;
using Tracker.Services.Abstraction;
using Tracker.Services.Abstraction.Results;
using Tracker.Services.ApiClients;

namespace Tracker.Services;

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
}