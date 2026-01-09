using Microsoft.AspNetCore.Components;
using MudBlazor;
using Tracker.API.Requests;
using Tracker.Domain.Dtos;

namespace Tracker.WebApp.Components.Shared;

public partial class ServerLoadingTable <TItem>
{
    private MudTable<TItem>? _table;

    [Parameter, EditorRequired]
    public Func<PaginatedSearchRequest, Task<Paginated<TItem>>> DataProvider
    { get; set; } = default!;

    [Parameter] public RenderFragment? Toolbar { get; set; }
    [Parameter] public RenderFragment? Header { get; set; }

    [Parameter, EditorRequired]
    public RenderFragment<TItem> Row { get; set; } = default!;

    private string? _search;

    private async Task<TableData<TItem>> LoadServerData(
        TableState state,
        CancellationToken cancellationToken)
    {
        var request = new PaginatedSearchRequest
        {
            SearchQuery = _search,
            Page = state.Page + 1,
            AmountInPage = state.PageSize
        };

        var result = await DataProvider(request);

        return new TableData<TItem>
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