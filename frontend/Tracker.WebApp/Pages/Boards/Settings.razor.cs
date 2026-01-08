using FluentValidation;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.Board;
using Tracker.Domain.ValueObjects;
using Tracker.Services.Abstraction;
using Tracker.WebApp.Shared;

namespace Tracker.WebApp.Pages.Boards;

public partial class Settings
{
    [Parameter]
    public Guid BoardId { get; set; }

    [Inject] IBoardService BoardService { get; set; } = null!;
    [Inject] IResultNotifier Notifier { get; set; } = null!;
    private BoardFullDto? Board { get; set; }
    private UpdateBoardRequest? model;
    private UpdateBoardRequestValidator validator = new();
    private MudForm? _form;
    private bool isLoading = true;
    private bool isSubmitting = false;

    protected override async Task OnInitializedAsync()
    {
        await LoadBoard();
    }

    protected override async Task OnParametersSetAsync()
    {
        if (Board == null || Board.Id != BoardId)
        {
            await LoadBoard();
        }
    }
    private async Task LoadBoard()
    {
        isLoading = true;
        try
        {
            var result = await BoardService.GetBoardByIdAsync(BoardId);
            if (result.IsFailure)
            {
                return;
            }

            Board = result.Value;
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
            StateHasChanged();
        }
        finally
        {
            isLoading = false;
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

        var result = await BoardService.UpdateAsync(BoardId, model);
        Notifier.Notify(result);
        isSubmitting = false;
    }

    private bool IsDisabled()
    {
        if (Board is null)
        {
            return false;
        }
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

    private string PageTitle()
    {
        return Board?.Title ?? "Board";
    }
}