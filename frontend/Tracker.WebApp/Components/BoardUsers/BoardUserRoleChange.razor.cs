using Microsoft.AspNetCore.Components;
using MudBlazor;
using Tracker.Domain.Dtos;
using Tracker.Domain.Enums;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Components.BoardUsers;

public partial class BoardUserRoleChange
{
    [CascadingParameter]
    private BoardState BoardState { get; set; } = null!;

    [Parameter]
    public BoardUserDto BoardUser { get; set; }

    private UserBoardRole _initialRole;
    private UserBoardRole _currentRole;
    private BoardUsersState Users => BoardState.Users;
    private bool HasChanged => _initialRole != _currentRole;

    protected override void OnInitialized()
    {
        BoardState.OnChange += StateHasChanged;
    }

    protected override void OnParametersSet()
    {
        _initialRole = BoardUser.Role;
        _currentRole = BoardUser.Role;
    }

    private async Task SubmitAsync()
    {
        if (!HasChanged)
        {
            return;
        }
        await Users.ChangeRoleAsync(BoardUser, _currentRole);
    }
}