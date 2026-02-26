using Refit;
using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.Board;

namespace Tracker.Services.ApiClients;

public interface IBoardsApi
{
    [Get("/api/boards/{id}")]
    Task<IApiResponse<BoardFullDto>> GetByIdAsync(Guid id);

    [Put("/api/boards/{id}/settings")]
    Task<IApiResponse> UpdateAsync(Guid id, [Body] UpdateBoardRequest request);

    [Delete("/api/boards/{id}")]
    Task<IApiResponse> DeleteAsync(Guid id);

    [Get("/api/boards/my")]
    Task<IApiResponse<List<BoardSummaryDto>>> GetForCurrentUserAsync();

    [Post("/api/boards/{id}/call")]
    Task<IApiResponse<Guid>> StartCallAsync(Guid id);

    [Put("/api/boards/{id}/archive")]
    Task<IApiResponse> ArchiveAsync(Guid id);

    [Put("/api/boards/{id}/unarchive")]
    Task<IApiResponse> UnarchiveAsync(Guid id);
}