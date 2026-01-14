using Microsoft.AspNetCore.Components;
using MudBlazor;
using Tracker.Domain.Dtos;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Components.BoardUsers;

public partial class BoardUserActions
{
    [CascadingParameter]
    private BoardState BoardState { get; set; } = null!;

    [Parameter, EditorRequired]
    public BoardUserDto BoardUser { get; set; }

    protected override void OnInitialized()
    {
        BoardState.OnChange += StateHasChanged;
    }

    private bool CanChange()
    {
        return BoardState.Board.Permissions.CanChangeBoard &&
            BoardUser.Role != Domain.Enums.UserBoardRole.Owner;
    }

}