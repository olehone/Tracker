using Microsoft.AspNetCore.Components;
using MudBlazor;
using Tracker.Domain.Dtos;

namespace Tracker.WebApp.Components.Users;

public partial class BriefUserInfo
{
    [Parameter, EditorRequired]
    public UserDto User { get; set; } = null!;
    [Parameter]
    public Size Size { get; set; } = Size.Medium;
}