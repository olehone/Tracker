using Microsoft.AspNetCore.Components;
using Tracker.Domain.Dtos;
using Tracker.Services.Abstraction.Auth;

namespace Tracker.WebApp.Components.Users;

public partial class UserInfo
{
    [Parameter]
    public required UserDto User { get; set; }

    [Inject] ICurrentUser CurrentUser { get; set; } = null!;
}