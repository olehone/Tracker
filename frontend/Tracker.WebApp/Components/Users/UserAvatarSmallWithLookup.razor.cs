using Microsoft.AspNetCore.Components;
using MudBlazor;
using Tracker.Domain.Dtos;

namespace Tracker.WebApp.Components.Users;

public partial class UserAvatarSmallWithLookup
{
    [Parameter]
    public required UserDto User { get; set; } = null!;
    [Parameter]
    public Color Color { get; set; } = Color.Default;
    [Parameter]
    public RenderFragment? AdditionalActions { get; set; }

    private bool _open;
    private bool _isHovering;

    private async Task HandleHovering(bool isHovering)
    {
        _isHovering = isHovering;

        await Task.Delay(300);

        _open = _isHovering;
        StateHasChanged();
    }
}