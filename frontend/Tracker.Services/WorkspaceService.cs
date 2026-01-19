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
    public Task<Result<WorkspaceFullDto>> GetByIdAsync(Guid id)
    {
        return apiErrorHandler.ExecuteAsync(() => api.GetByIdAsync(id));
    }

    public Task<Result> UpdateAsync(Guid id, UpdateWorkspaceRequest request)
    {
        return apiErrorHandler.ExecuteAsync(() => api.UpdateAsync(id, request));
    }

    public Task<Result<List<WorkspaceSummaryDto>>> GetForCurrentUserAsync()
    {
        return apiErrorHandler.ExecuteAsync(api.GetForCurrentUserAsync);
    }

    public Task<Result<WorkspaceSummaryDto>> CreateAsync(CreateWorkspaceRequest request)
    {
        return apiErrorHandler.ExecuteAsync(() => api.CreateAsync(request));
    }

    public Task<Result<Paginated<WorkspaceSummaryDto>>> GetAsync(PaginatedSearchRequest request)
    {
        return apiErrorHandler.ExecuteAsync(() => api.GetAsync(request));
    }
}