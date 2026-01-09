using Microsoft.AspNetCore.Components;
using Tracker.Domain.Dtos;
using Tracker.Services.Abstraction;

namespace Tracker.WebApp.Pages.Users;
public partial class Profile
{
    [Parameter]
    public Guid UserId { get; set; }

    [Inject] IUserService UserService { get; set; } = null!;

    private UserDto? User { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await LoadUser();
    }

    protected override async Task OnParametersSetAsync()
    {
        if (User == null || User.Id != UserId)
        {
            await LoadUser();
        }
    }
    private async Task LoadUser()
    {
        var result = await UserService.GetUserByIdAsync(UserId);
        if (result.IsFailure)
        {
            return;
        }

        User = result.Value;
        StateHasChanged();
    }

}