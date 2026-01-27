using Microsoft.AspNetCore.Components;
using MudBlazor;
using Tracker.Domain.Dtos;
using Tracker.Domain.Requests;

namespace Tracker.WebApp.Components.Workspaces;

public partial class WorkspaceList
{
    [Parameter, EditorRequired]
    public Func<PaginatedSearchRequest, Task<Paginated<WorkspaceSummaryDto>>> LoadWorkspaces { get; set; }
    [Parameter]
    public string? Title { get; set; }
    [Parameter]
    public string? Height { get; set; }

    private Variant SearchVariant => Title is null 
        ? Variant.Outlined 
        : Variant.Text;
    
    private MudTable<WorkspaceSummaryDto>? _table;

    private string? _search;

    private async Task<TableData<WorkspaceSummaryDto>> LoadServerData(
        TableState state,
        CancellationToken cancellationToken)
    {
        var request = new PaginatedSearchRequest
        {
            SearchQuery = _search,
            Page = state.Page + 1,
            AmountInPage = state.PageSize
        };

        var result = await LoadWorkspaces(request);

        return new TableData<WorkspaceSummaryDto>
        {
            TotalItems = result.TotalCount,
            Items = result.Items
        };
    }
    public void SetSearch(string? value)
    {
        _search = value;
        _table?.ReloadServerData();
    }
}