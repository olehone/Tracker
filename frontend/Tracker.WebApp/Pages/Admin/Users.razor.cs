using Microsoft.AspNetCore.Components;
using Tracker.API.Requests;
using Tracker.Domain.Dtos;
using Tracker.Services.Abstraction;
using Tracker.WebApp.Components.Shared;
using Tracker.WebApp.Shared;

namespace Tracker.WebApp.Pages.Admin;
public partial class Users
{
    [Inject] IUserService UserService { get; set; } = null!;
    [Inject] IErrorNotifier ErrorNotifier { get; set; } = null!;
    [Inject] NavigationManager Nav { get; set; } = null!;

    private ServerLoadingTable<UserDto>? _table;

    private async Task<Paginated<UserDto>> LoadUsers(
        PaginatedSearchRequest request)
    {
        var result = await UserService.GetUsersAsync(request);
        if (ErrorNotifier.NotifyIfError(result))
        {
            return Paginated<UserDto>.Empty();
        }
        return result.Value;
    }

    private void OnSearchChanged(string search)
    {
        _table?.SetSearch(search);
    }
}