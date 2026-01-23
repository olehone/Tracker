using Microsoft.AspNetCore.Mvc;
using Refit;
using Tracker.API.Requests;
using Tracker.Domain.Dtos;
using Tracker.Domain.Requests;
using Tracker.Domain.Requests.Workspace;

namespace Tracker.Services.ApiClients;

public interface IWorkspaceApi
{

    [Get("/api/workspaces/{id}")]
    Task<ApiResponse<WorkspaceFullDto>> GetByIdAsync(Guid id);

    [Put("/api/workspaces/{workspaceId}/settings")]
    Task<ApiResponse<object>> UpdateAsync(Guid workspaceId, [Body] UpdateWorkspaceRequest request);

    [Get("/api/workspaces/all")]
    Task<ApiResponse<Paginated<WorkspaceSummaryDto>>> GetAsync([Query] PaginatedSearchRequest request);

    [Get("/api/workspaces/my")]
    Task<ApiResponse<List<WorkspaceSummaryDto>>> GetForCurrentUserAsync();

    [Post("/api/workspaces")]
    Task<ApiResponse<WorkspaceSummaryDto>> CreateAsync(CreateWithTitleRequest request);

    [Post("/api/workspaces/{workspaceId}/boards")]
    Task<ApiResponse<BoardSummaryDto>> CreateBoardAsync(Guid workspaceId, CreateWithTitleRequest request);
}