using FluentValidation;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.Board;
using Tracker.Domain.ValueObjects;
using Tracker.Services.Abstraction;
using Tracker.WebApp.Shared;

namespace Tracker.WebApp.Components.Boards;

public partial class BoardMembersDialog
{
    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; } = null!;

    [Parameter]
    public required BoardFullDto Board { get; set; }

    [Inject] IBoardService BoardService { get; set; } = null!;
    [Inject] IResultNotifier Notifier { get; set; } = null!;

    private MudForm? _form;
    private UpdateBoardRequest model = null!;
    private readonly UpdateBoardRequestValidator validator = new();
    private bool isSubmitting = false;

    protected override void OnParametersSet()
    {
        model = new UpdateBoardRequest
        {
            Title = Board.Title,
            Description = Board.Description,
            Visibility = Board.Visibility,
            PermissionRoles = new BoardPermissionRoles
            {
                MinCreateItemRole = Board.PermissionRoles.MinCreateItemRole,
                MinChangeItemRole = Board.PermissionRoles.MinChangeItemRole,
                MinCreateListRole = Board.PermissionRoles.MinCreateListRole,
                MinChangeListRole = Board.PermissionRoles.MinChangeListRole,
            }
        };
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

        var result = await BoardService.UpdateAsync(Board.Id, model);
        Notifier.Notify(result);
        isSubmitting = false;

        if (result.IsSuccess)
        {
            MudDialog.Close(DialogResult.Ok(true));
        }
    }


    private void Cancel()
    {
        MudDialog.Cancel();
    }

    private bool IsDisabled()
    {
        return !Board.Permissions.CanChangeBoard;
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