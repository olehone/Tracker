using Microsoft.AspNetCore.Components;
using MudBlazor;
using Tracker.API.Requests;
using Tracker.Domain.Dtos;

namespace Tracker.WebApp.Components.Users;

public partial class UserListSummary
{
    [Parameter, EditorRequired]
    public Func<PaginatedSearchRequest, Task<Paginated<UserDto>>> LoadUsers { get; set; }

    [Parameter]
    public EventCallback<UserDto> OnRowClick { get; set; }

    private MudTable<UserDto>? _table;

    private string? _search;

    private async Task<TableData<UserDto>> LoadServerData(
        TableState state,
        CancellationToken cancellationToken)
    {
        if (_search is null || _search.Length < 1)
        {
            return new TableData<UserDto>
            {
                Items = [],
                TotalItems = 0,
            };
        }
        var request = new PaginatedSearchRequest
        {
            SearchQuery = _search,
            Page = 1,
            AmountInPage = 5
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