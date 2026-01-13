using Microsoft.AspNetCore.Components;
using Tracker.Domain.Enums;

namespace Tracker.WebApp.Components.BoardUsers;

public partial class BoardRoleSelect
{
    [Parameter]
    public UserBoardRole Value { get; set; }
    [Parameter]
    public EventCallback<UserBoardRole> ValueChanged { get; set; }
}