using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.Board;
using Tracker.Domain.Requests.Common;
using Tracker.Domain.Results;
using Tracker.Services.Abstraction;
using Tracker.Services.Abstraction.Results;
using Tracker.Services.ApiClients;

namespace Tracker.Services;

public class BoardService(IApiErrorHandler apiErrorHandler, IBoardsApi api) : IBoardService
{
    public Task<Result<BoardSummaryDto>> CreateBoardAsync(CreateBoardRequest request)
    {
        return apiErrorHandler.ExecuteAsync(() => api.CreateBoardAsync(request));
    }

    public Task<Result> UpdateAsync(Guid id, UpdateBoardRequest request)
    {
        return apiErrorHandler.ExecuteAsync(() => api.UpdateAsync(id, request));
    }

    public Task<Result<BoardFullDto>> GetBoardByIdAsync(Guid id)
    {
        return apiErrorHandler.ExecuteAsync(() => api.GetBoardByIdAsync(id));
    }
}