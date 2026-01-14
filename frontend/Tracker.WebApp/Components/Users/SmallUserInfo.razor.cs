using Microsoft.AspNetCore.Components;
using Tracker.Domain.Dtos;

namespace Tracker.WebApp.Components.Users;

public partial class SmallUserInfo
{
    [Parameter, EditorRequired]
    public UserDto User { get; set; } = null!;
}