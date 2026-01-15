using Microsoft.AspNetCore.Components;
using Tracker.Domain.Dtos;

namespace Tracker.WebApp.Components.BoardUsers;

public partial class BoardUserListItem
{
    [Parameter, EditorRequired]
    public BoardUserDto BoardUser { get; set; } = null!;
}