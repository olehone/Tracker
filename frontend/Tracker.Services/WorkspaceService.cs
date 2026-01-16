using Tracker.API.Requests;
using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.Workspace;
using Tracker.Domain.Results;
using Tracker.Services.Abstraction;
using Tracker.Services.Abstraction.Results;
using Tracker.Services.ApiClients;

namespace Tracker.Services;

public class WorkspaceService(IApiErrorHandler apiErrorHandler, IWorkspaceApi api)
    : IWorkspaceService
{
    public Task<Result<WorkspaceFullDto>> GetWorkspaceByIdAsync(Guid id)
    {
        return apiErrorHandler.ExecuteAsync(() => api.GetWorkspaceByIdAsync(id));
    }

    public Task<Result> UpdateAsync(Guid id, UpdateWorkspaceRequest request)
    {
        return apiErrorHandler.ExecuteAsync(() => api.UpdateAsync(id, request));
    }

    public Task<Result<List<WorkspaceSummaryDto>>> GetWorkspacesForCurrentUserAsync()
    {
        return apiErrorHandler.ExecuteAsync(api.GetWorkspacesForCurrentUserAsync);
    }

    public Task<Result<WorkspaceSummaryDto>> CreateWorkspaceAsync(CreateWorkspaceRequest request)
    {
        return apiErrorHandler.ExecuteAsync(() => api.CreateWorkspaceAsync(request));
    }

    public Task<Result<Paginated<WorkspaceSummaryDto>>> GetWorkspacesAsync(PaginatedSearchRequest request)
    {
        return apiErrorHandler.ExecuteAsync(() => api.GetWorkspacesAsync(request));
    }
}