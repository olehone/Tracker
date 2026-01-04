using Refit;
using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.Board;
using Tracker.Domain.Requests.Common;

namespace Tracker.Services.ApiClients;

public interface IBoardsApi
{
    [Get("/api/boards/{id}")]
    Task<ApiResponse<BoardFullDto>> GetBoardByIdAsync(GetByIdRequest request);

    [Post("/api/boards/")]
    Task<ApiResponse<BoardSummaryDto>> CreateBoardAsync(CreateBoardRequest request);
}