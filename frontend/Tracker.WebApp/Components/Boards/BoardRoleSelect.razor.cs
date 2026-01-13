using Microsoft.AspNetCore.Components;
using Tracker.Domain.Enums;

namespace Tracker.WebApp.Components.Boards;

public partial class BoardRoleSelect
{
    [Parameter]
    public UserBoardRole Value { get; set; }
    [Parameter]
    public EventCallback<UserBoardRole> ValueChanged { get; set; }
    [Parameter]
    public required string Label { get; set; }
    [Parameter]
    public required RenderFragment? Owner { get; set; }
}