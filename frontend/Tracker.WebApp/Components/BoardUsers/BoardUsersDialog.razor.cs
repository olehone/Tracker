using Microsoft.AspNetCore.Components;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Components.BoardUsers;

public partial class BoardUsersDialog
{
    [Parameter, EditorRequired]
    public BoardState BoardState { get; set; }
}