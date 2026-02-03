using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using Tracker.Domain.Dtos;
using Tracker.Services;
using Tracker.Services.Abstraction;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Components.Users;

public partial class UserInfo
{
    [Parameter]
    public required UserDto User { get; set; }

    [Inject] IUserService UserService { get; set; } = default!;
    [Inject] AppState AppState { get; set; } = default!;
    [Inject] ISnackbar Snackbar { get; set; } = default!;

    //TODO: add proper user permission check

    private async Task DeleteAvatarAsync()
    {
        await UserService.DeleteAvatarAsync(User.Id);
        User.AvatarUrl = null;
        StateHasChanged();
    }

    private async Task UploadAvatarAsync(IBrowserFile file)
    {
        if (file == null)
        {
            return;
        }

        await using var stream = file.OpenReadStream();
        var result = await UserService.UploadAvatarAsync(User.Id, stream, file.ContentType, file.Name);

        if (result.IsSuccess)
        {
            User.AvatarUrl = result.Value;
            if (AppState.IsAuthenticated && AppState.MyId == User.Id)
            {
                AppState.CurrentUser.AvatarUrl = result.Value;
            }
        }
        else
        {
            foreach (var detail in result.Error.Details!)
            {
                Snackbar.Add(detail, Severity.Warning);
            }
        }

        StateHasChanged();
    }
}