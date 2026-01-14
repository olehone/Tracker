using Tracker.Domain.Dtos;
using Tracker.Domain.Enums;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Components.BoardUsers;

public partial class AddBoardUserSection
{
    private UserDto? _selectedUser;
    private UserBoardRole _selectedRole = UserBoardRole.Observer;

    private async Task AddUser()
    {
        if (_selectedUser is null)
        {
            return;
        }
        await BoardState.Users.AddUserAsync(_selectedUser.Id, _selectedRole);
    }
}