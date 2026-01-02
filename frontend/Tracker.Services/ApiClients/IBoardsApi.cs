using Refit;
using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.Board;

namespace Tracker.Services.ApiClients;

public interface IBoardsApi
{
    [Get("/api/boards/{id}")]
    Task<BoardFullDto> GetBoardByIdAsync(Guid id);

    [Post("/api/boards/")]
    Task<BoardSummaryDto> CreateBoardAsync(CreateBoardRequest request);
}