using Microsoft.AspNetCore.Components;
using MudBlazor;
using Tracker.Domain.Dtos;
using Tracker.Domain.Requests;

namespace Tracker.WebApp.Components.Users;

public partial class UserListDetailed
{
    [Parameter, EditorRequired]
    public Func<PaginatedSearchRequest, Task<Paginated<UserDto>>> LoadUsers { get; set; }
    [Parameter]
    public string? Title { get; set; }

    private MudTable<UserDto>? _table;

    private string? _search;

    private async Task<TableData<UserDto>> LoadServerData(
        TableState state,
        CancellationToken cancellationToken)
    {
        var request = new PaginatedSearchRequest
        {
            SearchQuery = _search,
            Page = state.Page + 1,
            AmountInPage = state.PageSize
        };

        var result = await LoadUsers(request);

        return new TableData<UserDto>
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