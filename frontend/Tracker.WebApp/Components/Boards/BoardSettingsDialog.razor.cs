using FluentValidation;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.Board;
using Tracker.Domain.ValueObjects;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Components.Boards;

public partial class BoardSettingsDialog : IDisposable
{
    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; } = null!;

    [Parameter]
    public required BoardFullDto Board { get; set; }

    [Inject] private BoardState BoardState { get; set; } = null!;

    private MudForm? _form;
    private UpdateBoardRequest model = null!;
    private readonly UpdateBoardRequestValidator validator = new();
    private bool isSubmitting = false;

    protected override void OnInitialized()
    {
        BoardState.OnChange += StateHasChanged;

        if (BoardState.CurrentBoard != null)
        {
            model = new UpdateBoardRequest
            {
                Title = BoardState.CurrentBoard.Title,
                Description = BoardState.CurrentBoard.Description,
                Visibility = BoardState.CurrentBoard.Visibility,
                PermissionRoles = new BoardPermissionRoles
                {
                    MinCreateItemRole = BoardState.CurrentBoard.PermissionRoles.MinCreateItemRole,
                    MinChangeItemRole = BoardState.CurrentBoard.PermissionRoles.MinChangeItemRole,
                    MinCreateListRole = BoardState.CurrentBoard.PermissionRoles.MinCreateListRole,
                    MinChangeListRole = BoardState.CurrentBoard.PermissionRoles.MinChangeListRole,
                }
            };
        }
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
        StateHasChanged();

        var success = await BoardState.UpdateBoardAsync(model);

        isSubmitting = false;

        if (success)
        {
            MudDialog.Close(DialogResult.Ok(true));
        }
    }

    private void Cancel() => MudDialog.Cancel();

    private bool IsDisabled() =>
        BoardState.CurrentBoard?.Permissions.CanChangeBoard != true;

    void IDisposable.Dispose()
    {
        BoardState.OnChange -= StateHasChanged;
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
}