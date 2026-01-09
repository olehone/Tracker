using Microsoft.AspNetCore.Components;
using Tracker.Domain.Dtos;
using Tracker.Services.Abstraction;

namespace Tracker.WebApp.Components.Users;

public partial class UserProfileAvatar
{
    [Parameter]
    public UserDto User { get; set; } = null!;

    [Inject] private IAuthService AuthService { get; set; } = null!;
    [Inject] private NavigationManager Nav { get; set; } = null!;

    private bool Disabled()
    {
        return User is null;
    }
    private async Task Logout()
    {
        await AuthService.LogoutAsync();
        Nav.NavigateTo("/");
    }
    private void Profile()
    {
        Nav.NavigateTo($"/users/{User!.Id}");
    }
}