using Tracker.Domain.Dtos;
using Tracker.Domain.Enums;
using Tracker.Services.Abstraction;

namespace Tracker.WebApp.States;

public class AppState
{
    private readonly IUserService _userService;
    private readonly IAuthService _authService;

    private UserDto? _currentUser;
    private UserPermissionsDto? _permissions;

    public AppState(IUserService userService, IAuthService authService)
    {
        _userService = userService;
        _authService = authService;
    }

    public UserDto CurrentUser =>
        _currentUser ?? throw new InvalidOperationException("User accessed when unauthenticated.");

    public UserPermissionsDto Permissions =>
        _permissions ?? Empty;

    public Guid MyId => _currentUser!.Id;

    public bool IsLoading { get; private set; }
    public bool IsUnauthenticated => _currentUser is null;
    public bool IsAuthenticated => !IsUnauthenticated;

    public event Func<Task>? OnChange;

    public async Task ReloadAsync()
    {
        IsLoading = true;
        await NotifyAsync();

        try
        {
            var principal = await _authService.GetPrincipalAsync();

            if (principal.Identity?.IsAuthenticated is not true)
            {
                Clear();
                return;
            }

            var userResult = await _userService.GetCurrentAsync();
            if (userResult.IsFailure)
            {
                Clear();
                return;
            }

            var permissionsResult = await _userService.GetPermissionsAsync();
            if (permissionsResult.IsFailure)
            {
                Clear();
                return;
            }

            _currentUser = userResult.Value;
            _permissions = permissionsResult.Value;
        }
        finally
        {
            IsLoading = false;
            await NotifyAsync();
        }
    }

    public async Task ClearAsync()
    {
        Clear();
        await NotifyAsync();
    }

    private void Clear()
    {
        _currentUser = null;
        _permissions = null;
    }

    private async Task NotifyAsync()
    {
        if (OnChange is not null)
        {
            await OnChange.Invoke();
        }
    }

    private UserPermissionsDto Empty => new()
    {
        CurrentPlan = SubscriptionPlan.Free,
        CanSeeBoardCalendar = false,
        CanSeeBoardEisenhower = false,
        CanSeeBoardRoadmap = false,
        CanUseAi = false,
    };
}