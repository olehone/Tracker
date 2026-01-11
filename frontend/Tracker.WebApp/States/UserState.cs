using Tracker.Domain.Dtos;

namespace Tracker.WebApp.States;

public class UserState
{
    private UserDto? _currentUser;

    public event Action? OnUserChange;

    public UserDto? CurrentUser
    {
        get => _currentUser;
        set
        {
            _currentUser = value;
            NotifyUserChanged();
        }
    }

    public void NotifyUserChanged()
    {
        OnUserChange?.Invoke();
    }
}
