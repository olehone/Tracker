using FluentValidation;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.BoardList;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Components.BoardLists;

public partial class BoardListSettingsDialog : IDisposable
{
    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; } = null!;

    [Parameter]
    public BoardState BoardState { get; set; } = null!;
    [Parameter]
    public BoardListDto List { get; set; } = null!;

    [Inject] private IDialogService DialogService { get; set; }

    private MudForm? _form;
    private UpdateBoardListRequest model = null!;
    private readonly UpdateBoardListRequestValidator validator = new();
    private bool isSubmitting = false;

    protected override void OnInitialized()
    {
        BoardState.OnChange += StateHasChanged;

        if (BoardState.Board != null)
        {
            model = new UpdateBoardListRequest
            {
                Title = List.Title,
                Description = List.Description ?? string.Empty,
            };
        }
    }

    private async Task Delete()
    {
        bool? result = await DialogService.ShowMessageBox(
            "Warning",
            "Deleting can not be undone!",
            yesText: "Delete!", cancelText: "Cancel");
        if(result == null)
        {
            return;
        }
        await BoardState.DeleteBoardListAsync(List.Id);
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
        StateHasChanged();

        var success = await BoardState.UpdateBoardListAsync(List.Id, model);

        isSubmitting = false;

        if (success)
        {
            MudDialog.Close(DialogResult.Ok(true));
        }
    }

    private void Cancel() => MudDialog.Cancel();

    void IDisposable.Dispose()
    {
        BoardState.OnChange -= StateHasChanged;
    }

    public class UpdateBoardListRequestValidator : AbstractValidator<UpdateBoardListRequest>
    {
        public UpdateBoardListRequestValidator()
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