using Tracker.Domain.Dtos;

namespace Tracker.WebApp.States;

public class AppState(){

    private UserDto? _currentUser;
    
    public UserDto CurrentUser
    {
        get => _currentUser ?? throw new InvalidOperationException("User accessed before authentication");
        set
        {
            _currentUser = value;
            IsLoading = false;
            NotifyUserChanged();
        }
    }

    public bool IsUnauthenticated => _currentUser?.Id is null;
    public bool IsAuthenticated => !IsUnauthenticated;
    public bool IsLoading { get; internal set; } = false;
    public Guid MyId => CurrentUser.Id;
    public event Action? OnUserChange;

    public void NotifyUserChanged()
    {
        OnUserChange?.Invoke();
    }

    public void Clear()
    {
        _currentUser = null;
        IsLoading = false;
        NotifyUserChanged();
    }
}
