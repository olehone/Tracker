using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.Board;
using Tracker.Services.Abstraction;
using Tracker.Services.ApiClients;

namespace Tracker.Services;

public class BoardService(IBoardsApi api) : IBoardService
{
    public Task<BoardSummaryDto> CreateBoardAsync(CreateBoardRequest request)
        => api.CreateBoardAsync(request);

    public Task<BoardFullDto> GetBoardByIdAsync(Guid id)
        => api.GetBoardByIdAsync(id);
}