using Microsoft.AspNetCore.Components;
using Tracker.Domain.Dtos;
using Tracker.Domain.Enums;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Components.BoardUsers;

public partial class AddBoardUserSection
{
    [CascadingParameter]
    private BoardState BoardState { get; set; } = null!;

    private UserDto? _selectedUser;
    private UserBoardRole _selectedRole = UserBoardRole.Observer;
    private BoardUsersState Users => BoardState.Users;

    protected override void OnInitialized()
    {
        BoardState.OnChange += StateHasChanged;
    }

    private async Task AddUser()
    {
        if (_selectedUser is null)
        {
            return;
        }
        await BoardState.Users.AddUserAsync(_selectedUser.Id, _selectedRole);
    }
}