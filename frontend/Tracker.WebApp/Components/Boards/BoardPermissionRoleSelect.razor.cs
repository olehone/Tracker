using Microsoft.AspNetCore.Components;
using Tracker.Domain.Enums;

namespace Tracker.WebApp.Components.Boards;

public partial class BoardPermissionRoleSelect
{
    [Parameter]
    public BoardPermissionRole Value { get; set; }
    [Parameter]
    public EventCallback<BoardPermissionRole> ValueChanged { get; set; }
    [Parameter]
    public required string Label { get; set; }
}