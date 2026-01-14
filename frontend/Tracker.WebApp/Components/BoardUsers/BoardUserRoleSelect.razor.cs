using Microsoft.AspNetCore.Components;
using Tracker.Domain.Enums;

namespace Tracker.WebApp.Components.BoardUsers;

public partial class BoardUserRoleSelect
{
    [Parameter]
    public UserBoardRole Role { get; set; }
    [Parameter]
    public EventCallback<UserBoardRole> RoleChanged { get; set; }
}