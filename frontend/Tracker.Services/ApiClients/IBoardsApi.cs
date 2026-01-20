using Refit;
using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.Board;

namespace Tracker.Services.ApiClients;

public interface IBoardsApi
{
    [Get("/api/boards/{id}")]
    Task<ApiResponse<BoardFullDto>> GetByIdAsync(Guid id);

    [Post("/api/boards/")]
    Task<ApiResponse<BoardSummaryDto>> CreateAsync(CreateBoardRequest request);

    [Put("/api/boards/{id}/settings")]
    Task<ApiResponse<object>> UpdateAsync(Guid id, [Body] UpdateBoardRequest request);

    [Delete("/api/boards/{id}")]
    Task<ApiResponse<object>> DeleteAsync(Guid id);

    [Get("/api/boards/my")]
    Task<ApiResponse<List<BoardSummaryDto>>> GetForCurrentUserAsync();
}