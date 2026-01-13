using Microsoft.AspNetCore.Components;
using Tracker.Domain.Enums;

namespace Tracker.WebApp.Components.BoardUsers;

public partial class BoardRoleChange
{
    [Parameter]
    public UserBoardRole Role { get; set; }
    [Parameter]
    public EventCallback<UserBoardRole> OnSubmit { get; set; }

    protected override void OnParametersSet()
    {
        _initialRole = Role;
        _currentRole = Role;
    }
    
    private UserBoardRole _initialRole;
    private UserBoardRole _currentRole;

    private bool HasChanged => _initialRole != _currentRole;

    private async Task SubmitAsync()
    {
        if (!HasChanged)
        {
            return;
        }
        await OnSubmit.InvokeAsync(_currentRole);
        _initialRole = _currentRole;
    }
}