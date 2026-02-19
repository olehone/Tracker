using Microsoft.AspNetCore.Components;
using Tracker.Domain.Dtos;
using Tracker.Domain.Requests;
using Tracker.Services.Abstraction.Results;
using Tracker.Services.Abstraction.Workspace;

namespace Tracker.WebApp.Pages.Admin;

public partial class Workspaces
{
    [Inject] IWorkspaceService WorkspaceService { get; set; } = null!;
    [Inject] IErrorNotifier ErrorNotifier { get; set; } = null!;

    private async Task<Paginated<WorkspaceSummaryDto>> LoadWorkspaces(
        PaginatedSearchRequest request)
    {
        var result = await WorkspaceService.GetAsync(request);
        if (ErrorNotifier.NotifyIfError(result))
        {
            return Paginated<WorkspaceSummaryDto>.Empty();
        }
        return result.Value;
    }
}