using Microsoft.AspNetCore.Components;
using Tracker.Domain.Dtos;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Components.BoardUsers;

public partial class BoardUserActions
{
    [Parameter, EditorRequired]
    public BoardUserDto BoardUser { get; set; }

    private bool CanChange()
    {
        return BoardState.Board.Permissions.CanChangeBoard &&
            BoardUser.Role != Domain.Enums.UserBoardRole.Owner;
    }
}