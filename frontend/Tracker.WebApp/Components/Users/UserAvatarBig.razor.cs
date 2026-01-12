using Microsoft.AspNetCore.Components;
using Tracker.Domain.Dtos;
using Tracker.WebApp.Shared;

namespace Tracker.WebApp.Components.Users;

public partial class UserAvatarBig
{
    [Parameter]

    public required UserDto User { get; set; }

    private string CustomColor
    {
        get
        {
            _customColor ??= UiHelper.GetColorByString(User.Id);
            return _customColor;
        }
    }
    private string? _customColor;


    private char FirstLetter()
    {
        return User.Username.ToUpper()[0];
    }
}