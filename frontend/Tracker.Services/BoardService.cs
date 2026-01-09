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
        return apiErrorHandler.ExecuteAsync(request, api.CreateBoardAsync);
    }

    public Task<Result> UpdateAsync(GetByIdRequest id, UpdateBoardRequest request)
    {
        return apiErrorHandler.ExecuteAsync(id.Id, request, api.UpdateAsync);
    }

    public Task<Result<BoardFullDto>> GetBoardByIdAsync(GetByIdRequest request)
    {
        return apiErrorHandler.ExecuteAsync(request.Id, api.GetBoardByIdAsync);
    }
}