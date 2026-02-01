using MudBlazor;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Tracker.Domain.Requests.Board;
using Tracker.Domain.ValueObjects;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Components.Boards;

public partial class BoardSettingsDialog : IDisposable
{
    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; } = null!;
    [Parameter]
    public BoardState BoardState { get; set; } = null!;

    [Inject] IDialogService DialogService { get; set; } = null!;

    private MudForm? _form;
    private UpdateBoardRequest model = null!;
    private readonly UpdateBoardRequestValidator validator = new();
    private bool isSubmitting = false;
    private bool _disposed;

    protected override void OnInitialized()
    {
        BoardState.OnChange += StateHasChangedHandler;

        model = new UpdateBoardRequest
        {
            Title = BoardState.Board.Title,
            Description = BoardState.Board.Description,
            Visibility = BoardState.Board.Visibility,
            PermissionRoles = new BoardPermissionRoles
            {
                MinCreateItemRole = BoardState.Board.PermissionRoles.MinCreateItemRole,
                MinChangeItemRole = BoardState.Board.PermissionRoles.MinChangeItemRole,
                MinCreateListRole = BoardState.Board.PermissionRoles.MinCreateListRole,
                MinChangeListRole = BoardState.Board.PermissionRoles.MinChangeListRole,
            }
        };
    }

    private async Task Delete()
    {
        bool? result = await DialogService.ShowMessageBox(
            "Warning",
            "Deleting can not be undone!",
            yesText: "Delete!", cancelText: "Cancel");
        if (result == null)
        {
            return;
        }
        await BoardState.DeleteBoardAsync();
        MudDialog.Close(DialogResult.Ok(true));
    }

    private async Task Submit()
    {
        if (_form is null || model is null)
        {
            return;
        }

        await _form.Validate();
        if (!_form.IsValid)
        {
            return;
        }

        isSubmitting = true;

        await InvokeAsync(StateHasChanged);
        await BoardState.UpdateBoardAsync(model);

        isSubmitting = false;
        MudDialog.Close(DialogResult.Ok(true));
    }

    public class UpdateBoardRequestValidator : AbstractValidator<UpdateBoardRequest>
    {
        public UpdateBoardRequestValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required")
                .MinimumLength(3)
                .MaximumLength(100).WithMessage("Title can't exceed 100 characters");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description can't exceed 500 characters");

            RuleFor(x => x.Visibility)
                .IsInEnum().WithMessage("Invalid visibility");

            RuleFor(x => x.PermissionRoles)
                .NotNull().WithMessage("Permission roles are required");
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                BoardState.OnChange -= StateHasChangedHandler;
            }
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private void Cancel() => MudDialog.Cancel();

    private bool IsDisabled() =>
        BoardState.Board?.Permissions.CanChangeBoard != true;

    private void StateHasChangedHandler()
    {
        InvokeAsync(StateHasChanged);
    }
}