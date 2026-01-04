using Refit;
using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.Board;

namespace Tracker.Services.ApiClients;

public interface IBoardsApi
{
    [Get("/api/boards/{id}")]
    Task<ApiResponse<BoardFullDto>> GetBoardByIdAsync(Guid id);

    [Post("/api/boards/")]
    Task<ApiResponse<BoardSummaryDto>> CreateBoardAsync(CreateBoardRequest request);
}