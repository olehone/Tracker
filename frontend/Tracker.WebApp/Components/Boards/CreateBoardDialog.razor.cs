using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.Board;
using Tracker.Services.Abstraction;

namespace Tracker.WebApp.Components.Boards;
public partial class CreateBoardDialog
{
    private MudForm? _form;
    private bool _isValid;
    private bool _processing = false;
    private CreateBoardRequest _request = new();

    [Parameter]
    public bool ShowDialog { get; set; }

    [Parameter]
    public EventCallback<bool> ShowDialogChanged { get; set; }

    [Parameter]
    public WorkspaceDto? Workspace { get; set; }

    [Parameter]
    public EventCallback<BoardSummaryDto> OnBoardCreated { get; set; }

    [Inject]
    private IBoardService BoardService { get; set; } = default!;

    [Inject]
    private NavigationManager Navigation { get; set; } = default!;

    protected override void OnParametersSet()
    {
        if (ShowDialog && Workspace != null)
        {
            _request = new CreateBoardRequest { WorkspaceId = Workspace.Id };
        }
    }

    private async Task Cancel()
    {
        ShowDialog = false;
        await ShowDialogChanged.InvokeAsync(false);
    }

    private async Task Submit()
    {
        _processing = true;
        await _form!.Validate();

        if (!_isValid)
        {
            _processing = false;
            return;
        }

        var created = await BoardService.CreateBoardAsync(_request);

        if (created is not null)
        {
            await OnBoardCreated.InvokeAsync(created);
            Navigation.NavigateTo($"/boards/{created.Id}");
        }

        _processing = false;
        ShowDialog = false;
        await ShowDialogChanged.InvokeAsync(false);
        StateHasChanged();
    }
}