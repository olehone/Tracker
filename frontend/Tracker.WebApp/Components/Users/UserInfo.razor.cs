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

    private bool _isUploading = false;

    private async Task UploadAvatarAsync(IBrowserFile file)
    {
        if (file == null)
        {
            return;
        }

        var allowedTypes = new[] { "image/png", "image/jpeg", "image/jpg" };
        if (!allowedTypes.Contains(file.ContentType))
        {
            Snackbar.Add("Please upload a PNG or JPEG image", Severity.Warning);
            return;
        }

        const long maxFileSize = 5 * 1024 * 1024;
        if (file.Size > maxFileSize)
        {
            Snackbar.Add("File size must be less than 5MB", Severity.Warning);
            return;
        }

        _isUploading = true;
        StateHasChanged();

        await using var stream = file.OpenReadStream(maxFileSize);
        var result = await UserService.UploadAvatarAsync(User.Id, stream, file.Name);

        if (result.IsSuccess)
        {
            Snackbar.Add("Avatar uploaded successfully", Severity.Success);
            var newUrl = result.Value + "?new=true";
            User.AvatarUrl = newUrl;
            if (AppState.IsAuthenticated && AppState.MyId == User.Id)
            {
                AppState.CurrentUser.AvatarUrl = newUrl;
            }
        }
        _isUploading = false;
        StateHasChanged();
    }
}