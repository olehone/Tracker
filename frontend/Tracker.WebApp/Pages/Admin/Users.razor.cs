using Microsoft.AspNetCore.Components;
using Tracker.Domain.Dtos;
using Tracker.Domain.Requests;
using Tracker.Services.Abstraction;
using Tracker.Services.Abstraction.Results;

namespace Tracker.WebApp.Pages.Admin;
public partial class Users
{
    [Inject] IUserService UserService { get; set; } = null!;
    [Inject] IErrorNotifier ErrorNotifier { get; set; } = null!;

    private async Task<Paginated<UserDto>> LoadUsers(
        PaginatedSearchRequest request)
    {
        var result = await UserService.GetAsync(request);
        if (ErrorNotifier.NotifyIfError(result))
        {
            return Paginated<UserDto>.Empty();
        }
        return result.Value;
    }
}