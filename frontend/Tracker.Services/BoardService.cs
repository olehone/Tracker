using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.Board;
using Tracker.Domain.Requests.Common;
using Tracker.Domain.Results;
using Tracker.Services.Abstraction.Entities;
using Tracker.Services.Abstraction.Results;
using Tracker.Services.ApiClients;

namespace Tracker.Services.Entities;

public class BoardService(IApiErrorHandler apiErrorHandler, IBoardsApi api) : IBoardService
{
    public Task<Result<BoardSummaryDto>> CreateBoardAsync(CreateBoardRequest request)
        => apiErrorHandler.ExecuteAsync(request, api.CreateBoardAsync);

    public Task<Result<BoardFullDto>> GetBoardByIdAsync(GetByIdRequest request)
        => apiErrorHandler.ExecuteAsync(request.Id, api.GetBoardByIdAsync);
}