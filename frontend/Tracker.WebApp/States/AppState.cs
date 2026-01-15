using Tracker.Domain.Dtos;

namespace Tracker.WebApp.States;

public class AppState(){

    private UserDto? _currentUser;
    public UserDto? CurrentUser
    {
        get => _currentUser;
        set
        {
            _currentUser = value;
            NotifyUserChanged();
        }
    }
    public event Action? OnUserChange;

    public void NotifyUserChanged()
    {
        OnUserChange?.Invoke();
    }
}
