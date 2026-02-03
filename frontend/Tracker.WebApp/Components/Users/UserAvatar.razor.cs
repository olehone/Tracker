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
    public Color Color { get; set; } = Color.Default;
    [Parameter]
    public EventCallback<bool> HandleHovering { get; set; }

    private bool _avatarFailed;

    private void HandleImageError()
    {
        Console.WriteLine("Error on loading image");
        _avatarFailed = true;
        StateHasChanged();
    }

    private string CustomColor
    {
        get
        {
            _customColor ??= UiHelper.GetColorById(User.Id);
            return _customColor;
        }
    }

    private string? _customColor;

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