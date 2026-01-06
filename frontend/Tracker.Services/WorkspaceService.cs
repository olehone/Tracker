using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.Common;
using Tracker.Domain.Requests.Workspace;
using Tracker.Domain.Results;
using Tracker.Services.Abstraction;
using Tracker.Services.Abstraction.Results;
using Tracker.Services.ApiClients;

namespace Tracker.Services;

public class WorkspaceService(IApiErrorHandler apiErrorHandler, IWorkspaceApi api)
    : IWorkspaceService
{
    public Task<Result<WorkspaceFullDto>> GetWorkspaceByIdAsync(GetByIdRequest request)
    {
        return apiErrorHandler.ExecuteAsync(request.Id, api.GetWorkspaceByIdAsync);
    }

    public Task<Result<List<WorkspaceSummaryDto>>> GetWorkspacesForCurrentUserAsync()
    {
        return apiErrorHandler.ExecuteAsync(api.GetWorkspacesForCurrentUserAsync);
    }

    public Task<Result<WorkspaceSummaryDto>> CreateWorkspaceAsync(CreateWorkspaceRequest request)
    {
        return apiErrorHandler.ExecuteAsync(request, api.CreateWorkspaceAsync);
    }

}