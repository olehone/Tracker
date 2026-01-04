using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.Common;
using Tracker.Domain.Requests.Workspace;
using Tracker.Domain.Results;
using Tracker.Services.Abstraction.Entities;
using Tracker.Services.Abstraction.Results;
using Tracker.Services.ApiClients;

namespace Tracker.Services.Entities;

public class WorkspaceService(IApiErrorHandler apiErrorHandler, IWorkspaceApi api)
    : IWorkspaceService
{
    public Task<Result<List<WorkspaceDto>>> GetWorkspacesAsync()
    {
        return apiErrorHandler.ExecuteAsync(api.GetWorkspacesAsync);
    }

    public Task<Result<WorkspaceDto>> GetWorkspaceByIdAsync(GetByIdRequest request)
    {
        return apiErrorHandler.ExecuteAsync(request.Id, api.GetWorkspaceByIdAsync);
    }

    public Task<Result<WorkspaceDto>> CreateWorkspaceAsync(CreateWorkspaceRequest request)
    {
        return apiErrorHandler.ExecuteAsync(request, api.CreateWorkspaceAsync);
    }
}