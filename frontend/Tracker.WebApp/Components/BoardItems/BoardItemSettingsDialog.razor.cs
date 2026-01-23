using FluentValidation;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.BoardItem;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Components.BoardItems;

public partial class BoardItemSettingsDialog : IDisposable
{
    private static readonly UpdateBoardItemRequestValidator Validator = new();

    private MudForm? _form;
    private UpdateBoardItemRequest _model = null!;
    private bool _isSubmitting;
    private bool _openAssign;
    private bool _disposed;

    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; } = null!;

    [Parameter]
    public BoardState BoardState { get; set; } = null!;
    [Parameter]
    public BoardItemDto Item { get; set; } = null!;


    private bool IsItemExists =>
        BoardState.ItemsState.BoardItems.Any(i => i.Id == Item.Id);

    private void ToggleAssign()
    {
        _openAssign = !_openAssign;
    }

    protected override void OnInitialized()
    {
        BoardState.ItemsState.OnChange += StateHasChanged;

        _model = new UpdateBoardItemRequest
        {
            Title = Item.Title,
            Description = Item.Description ?? string.Empty,
            IsDone = Item.IsDone,
        };
    }

    private async Task Submit()
    {
        if (_form is null)
        {
            return;
        }

        await _form.Validate();
        if (!_form.IsValid)
        {
            return;
        }

        _isSubmitting = true;
        StateHasChanged();

        await BoardState.ItemsState.UpdateAsync(Item.Id, _model);

        _isSubmitting = false;
        MudDialog.Close(DialogResult.Ok(true));
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            BoardState.ItemsState.OnChange -= StateHasChanged;
        }

        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public class UpdateBoardItemRequestValidator : AbstractValidator<UpdateBoardItemRequest>
    {
        public UpdateBoardItemRequestValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required")
                .MinimumLength(3)
                .MaximumLength(100).WithMessage("Title can't exceed 100 characters");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description can't exceed 500 characters");
        }
    }
}