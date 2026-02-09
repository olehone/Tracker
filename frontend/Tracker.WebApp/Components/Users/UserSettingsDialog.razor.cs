using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.Users;
using Tracker.Domain.Results;
using Tracker.Services.Abstraction;
using Tracker.WebApp.Shared;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Components.Users;

public partial class UserSettingsDialog
{
    private const int MaxAvatarSizeBytes = 5 * 1024 * 1024;

    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; } = null!;

    [Parameter, EditorRequired]
    public required UserDto User { get; set; }
    [Parameter, EditorRequired]
    public required AppState AppState { get; set; }

    [Parameter]
    public EventCallback<UserDto> UserChanged { get; set; }

    [Inject] IUserService UserService { get; set; } = null!;
    [Inject] IDialogService DialogService { get; set; } = null!;
    [Inject] ISnackbar Snackbar { get; set; } = null!;

    private UpdateUserRequest _model = null!;
    private readonly UpdateUserModelValidator _validator = new();
    private MudForm _form = null!;

    private bool _isValid;
    private bool _isLoading;
    private IReadOnlyList<string>? _errorMessages;

    protected override void OnInitialized()
    {
        _model = new UpdateUserRequest
        {
            Username = User.Username,
            FirstName = User.FirstName,
            LastName = User.LastName
        };
    }

    private async Task HandleSubmitAsync()
    {
        await _form.Validate();

        if (!_form.IsValid)
        {
            return;
        }

        _isLoading = true;
        _errorMessages = null;
        StateHasChanged();

        var result = await UserService.UpdateAsync(User.Id, _model);

        _isLoading = false;

        if (HandleError(result))
        {
            return;
        }

        User.Username = _model.Username;
        User.FirstName = _model.FirstName;
        User.LastName = _model.LastName;
        await UserChanged.InvokeAsync(User);
    }

    private async Task DeleteAvatarAsync()
    {

        var confirmed = await DialogService.ShowMessageBox(
            title: "Warning",
            message: "You are going to delete avatar",
            yesText: "Delete",
            cancelText: "Cancel",
            options: new DialogOptions { FullWidth = false });

        if (confirmed != true)
        {
            return;
        }

        var result = await UserService.DeleteAvatarAsync(User.Id);
        if (result.IsSuccess)
        {
            User.AvatarUpdatedAt = null;
            await UserChanged.InvokeAsync(User);
            MudDialog.Close(DialogResult.Ok(true));
        }
    }

    private async Task UploadAvatarAsync(IBrowserFile file)
    {
        if (!IsFileValid(file))
        {
            return;
        }

        await using var stream = file.OpenReadStream(MaxAvatarSizeBytes);
        var result = await UserService.UploadAvatarAsync(User.Id, stream, file.ContentType, file.Name);

        if (result.IsSuccess)
        {
            User.AvatarUpdatedAt = DateTimeOffset.UtcNow;
            await UserChanged.InvokeAsync(User);
            MudDialog.Close(DialogResult.Ok(true));
        }
    }

    private bool HandleError(Result result)
    {
        if (result.IsFailure)
        {
            var error = result.Error!;
            _errorMessages = error.Type == ErrorType.Validation
                ? error.Details
                : [error.Description];

            StateHasChanged();
            return true;
        }

        return false;
    }

    private void Cancel() => MudDialog.Cancel();

    private bool IsFileValid(IBrowserFile file)
    {
        if (file is null)
        {
            Snackbar.Add("Avatar is not selected", Severity.Warning);
            return false;
        }

        if (file.Size == 0)
        {
            Snackbar.Add("Avatar is empty", Severity.Warning);
            return false;
        }

        if (file.Size > MaxAvatarSizeBytes)
        {
            var size = UiHelper.FileSize(MaxAvatarSizeBytes);
            Snackbar.Add($"Avatar must be less than or equal to {size}", Severity.Warning);
            return false;
        }
        return true;
    }
}
