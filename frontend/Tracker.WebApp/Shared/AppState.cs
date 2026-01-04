using Tracker.Domain.Dtos;

namespace Tracker.WebApp.States;

public class AppState
{
    private UserDto? _currentUser;
    public UserDto? CurrentUser
    {
        get => _currentUser;
        set
        {
            _currentUser = value;
            NotifyStateChanged();
        }
    }
    public event Action? OnChange;
    public void NotifyStateChanged() => OnChange?.Invoke();
}
