using Tracker.Domain.Dtos;

namespace Tracker.WebApp.States;

public class AppState()
{
    private UserDto? _currentUser;

    public UserDto CurrentUser
    {
        get => _currentUser
            ?? throw new InvalidOperationException("User accessed when unauthenticated");
        set
        {
            _currentUser = value;
            NotifyUserChanged();
        }
    }

    public Guid MyId => _currentUser!.Id;
    public bool IsUnauthenticated => _currentUser is null;
    public bool IsAuthenticated => !IsUnauthenticated;

    public event Action? OnUserChange;

    public void StartLoading()
    {
        NotifyUserChanged();
    }

    public void Clear()
    {
        _currentUser = null;
        NotifyUserChanged();
    }

    private void NotifyUserChanged()
    {
        OnUserChange?.Invoke();
    }
}