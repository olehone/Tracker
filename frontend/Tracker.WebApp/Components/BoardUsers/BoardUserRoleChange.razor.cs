using Microsoft.AspNetCore.Components;
using Tracker.Domain.Dtos;
using Tracker.Domain.Enums;

namespace Tracker.WebApp.Components.BoardUsers;

public partial class BoardUserRoleChange : BoardUsersSubscribeBase
{
    [Parameter, EditorRequired]
    public BoardUserDto BoardUser { get; set; }
    
    private UserBoardRole _initialRole;
    private UserBoardRole _currentRole;
    private bool HasChanged => _initialRole != _currentRole;

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