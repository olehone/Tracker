using Refit;
using Tracker.Domain.Dtos;
using Tracker.Domain.Requests;
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

    public Task<Result<WorkspaceSummaryDto>> CreateAsync(string title)
    {
        return apiErrorHandler.ExecuteAsync(() => api.CreateAsync(
            new CreateWithTitleRequest { Title = title }));
    }

    public Task<Result<BoardSummaryDto>> CreateBoardAsync(Guid id, string title)
    {
        return apiErrorHandler.ExecuteAsync(() => api.CreateBoardAsync(id,
            new CreateWithTitleRequest { Title = title }));
    }

    public Task<Result<Paginated<WorkspaceSummaryDto>>> GetAsync(PaginatedSearchRequest request)
    {
        return apiErrorHandler.ExecuteAsync(() => api.GetAsync(request));
    }
}