using Microsoft.AspNetCore.Components;
using Tracker.Domain.Enums;

namespace Tracker.WebApp.Components.BoardUsers;

public partial class BoardUserRoleSelect
{
    [Parameter]
    public BoardUserRole Role { get; set; }
    [Parameter]
    public EventCallback<BoardUserRole> RoleChanged { get; set; }
}