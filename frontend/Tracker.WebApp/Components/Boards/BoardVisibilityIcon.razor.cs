using Microsoft.AspNetCore.Components;
using Tracker.Domain.Enums;

namespace Tracker.WebApp.Components.Boards;

public partial class BoardVisibilityIcon
{
    [Parameter]
    public BoardVisibility? Visibility { get; set; }
}