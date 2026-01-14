using Microsoft.AspNetCore.Components;
using Tracker.Domain.Enums;

namespace Tracker.WebApp.Components.BoardUsers;

public partial class BoardUserRoleChip
{
    [Parameter]
    public UserBoardRole Role { get; set; }
}