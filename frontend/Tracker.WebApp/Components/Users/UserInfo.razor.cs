using Microsoft.AspNetCore.Components;
using Tracker.Domain.Dtos;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Components.Users;
public partial class UserInfo
{
    [Parameter]
    public required UserDto User { get; set; }

    [Inject] AppState AppState { get; set; } = null!;
    private bool IsOwn()
    {
        return User.Id == AppState.CurrentUser?.Id;
    }
}