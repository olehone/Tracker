using Microsoft.AspNetCore.Components;
using Tracker.Domain.Enums;

namespace Tracker.WebApp.Components.BoardUsers;

public partial class BoardRoleChip
{
    [Parameter]
    public UserBoardRole Role { get; set; }
}