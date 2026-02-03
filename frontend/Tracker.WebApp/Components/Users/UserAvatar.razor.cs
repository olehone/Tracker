using Microsoft.AspNetCore.Components;
using MudBlazor;
using Tracker.Domain.Dtos;
using Tracker.WebApp.Shared;

namespace Tracker.WebApp.Components.Users;

public partial class UserAvatar
{
    [Parameter, EditorRequired]
    public UserDto User { get; set; }
    [Parameter]
    public Size Size { get; set; } = Size.Medium;
    [Parameter]
    public string Style { get; set; } = string.Empty;
    [Parameter]
    public EventCallback<bool> HandleHovering { get; set; }

    private bool _avatarFailed = false;
    private string? _customColor;

    private void HandleImageError()
    {
        _avatarFailed = true;
        StateHasChanged();
    }

    private string ImageStyle
    {
        get
        {
            _customColor ??= UiHelper.GetColorById(User.Id);
            return _customColor + Style;
        }
    }

    public string GetUserColor()
    {
        var value = UiHelper.GetColorById(User!.Id);
        return value;
    }

    private char FirstLetter()
    {
        return User.Username.ToUpper()[0];
    }
}