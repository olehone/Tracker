using Refit;
using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.Board;
using Tracker.Domain.Requests.Workspace;

namespace Tracker.Services.ApiClients;

public interface IBoardsApi
{
    [Get("/api/boards/{id}")]
    Task<ApiResponse<BoardFullDto>> GetBoardByIdAsync(Guid id);

    [Put("/api/boards/{id}/settings")]
    Task<ApiResponse<object>> UpdateAsync(Guid id, [Body] UpdateBoardRequest request);

    [Post("/api/boards/")]
    Task<ApiResponse<BoardSummaryDto>> CreateBoardAsync(CreateBoardRequest request);
}