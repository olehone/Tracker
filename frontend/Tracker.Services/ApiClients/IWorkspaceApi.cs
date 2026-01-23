using Refit;
using Tracker.API.Requests;
using Tracker.Domain.Dtos;
using Tracker.Domain.Requests;
using Tracker.Domain.Requests.Workspace;

namespace Tracker.Services.ApiClients;

public interface IWorkspaceApi
{

    [Get("/api/workspaces/{id}")]
    Task<IApiResponse<WorkspaceFullDto>> GetByIdAsync(Guid id);

    [Put("/api/workspaces/{workspaceId}/settings")]
    Task<IApiResponse> UpdateAsync(Guid workspaceId, [Body] UpdateWorkspaceRequest request);

    [Get("/api/workspaces/all")]
    Task<IApiResponse<Paginated<WorkspaceSummaryDto>>> GetAsync([Query] PaginatedSearchRequest request);

    [Get("/api/workspaces/my")]
    Task<IApiResponse<List<WorkspaceSummaryDto>>> GetForCurrentUserAsync();

    [Post("/api/workspaces")]
    Task<IApiResponse<WorkspaceSummaryDto>> CreateAsync(CreateWithTitleRequest request);

    [Post("/api/workspaces/{id}/boards")]
    Task<IApiResponse<BoardSummaryDto>> CreateBoardAsync(Guid id, CreateWithTitleRequest request);
}