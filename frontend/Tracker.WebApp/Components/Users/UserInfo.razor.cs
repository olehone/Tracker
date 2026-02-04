using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Tracker.Domain.Dtos;
using Tracker.Services.Abstraction;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Components.Users;

public partial class UserInfo
{
    [Parameter]
    public required UserDto User { get; set; }

    [Parameter]
    public EventCallback<UserDto> UserChanged { get; set; }

    [Inject] private AppState AppState { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;

    private bool IsMe => AppState.IsAuthenticated && AppState.MyId == User.Id;

    private async Task OpenSettingsAsync()
    {
        var parameters = new DialogParameters
        {
            { nameof(UserSettingsDialog.User), User },
            { nameof(UserSettingsDialog.UserChanged), EventCallback.Factory.Create<UserDto>(this, OnUserUpdated) }
        };

        var dialog = await DialogService.ShowAsync<UserSettingsDialog>(
            User.Username,
            parameters,
            new DialogOptions { NoHeader = true });

        await dialog.Result;
    }

    private async Task OnUserUpdated(UserDto updatedUser)
    {
        User = updatedUser;
        await UpdateUserStateAsync();
    }

    private async Task UpdateUserStateAsync()
    {
        if (IsMe)
        {
            var user = AppState.CurrentUser;
            user.AvatarUrl = User.AvatarUrl;
            user.Username = User.Username;
            user.FirstName = User.FirstName;
            user.LastName = User.LastName;
            AppState.CurrentUser = user;
        }

        await UserChanged.InvokeAsync(User);
        StateHasChanged();
    }
}
