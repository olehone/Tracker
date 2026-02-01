using Microsoft.AspNetCore.Components;
using Tracker.Domain.Dtos;

namespace Tracker.WebApp.Components.Users;

public partial class UserInfo
{
    [Parameter]
    public required UserDto User { get; set; }
}