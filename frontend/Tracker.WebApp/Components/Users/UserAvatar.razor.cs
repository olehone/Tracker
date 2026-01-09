using Microsoft.AspNetCore.Components;
using Tracker.Domain.Dtos;
using Tracker.Services.Abstraction;
using Tracker.WebApp.Shared;

namespace Tracker.WebApp.Components.Users;

public partial class UserAvatar
{
    [Parameter]
    public UserDto User { get; set; } = null!;

    [Inject] private IAuthService AuthService { get; set; } = null!;
    [Inject] private NavigationManager Nav { get; set; } = null!;

    private string CustomColor
    {
        get
        {
            _customColor ??= UiHelper.GetColorByString(User.Id);
            return _customColor;
        }
    }

    private string? _customColor;
    
    public string GetUserColor()
    {
        var value = UiHelper.GetColorByString(User!.Id);
        return value;
    }
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

    private char FirstLetter()
    {
        return User.Username.ToUpper()[0];
    }
}