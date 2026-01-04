using Microsoft.AspNetCore.Components;
using MudBlazor;
using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.Board;
using Tracker.Services.Abstraction.Entities;

namespace Tracker.WebApp.Components.Boards;
public partial class CreateBoardDialog
{
    private MudForm? _form;
    private bool _isValid;
    private bool _processing = false;
    //private CreateBoardRequest _request = new();

    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; } = null!;

    [Parameter]
    public WorkspaceDto Workspace { get; set; } = null!;

    [Inject] private IBoardService BoardService { get; set; } = null!;
    [Inject] private NavigationManager Navigation { get; set; } = null!;

    //protected override void OnInitialized()
    //{
    //    _request = new CreateBoardRequest { WorkspaceId = Workspace.Id };
    //}

    private void Cancel()
    {
        MudDialog.Cancel();
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

        //var created = await BoardService.CreateBoardAsync(_request);

        //if (created is not null)
        //{
        //    Navigation.NavigateTo($"/boards/{created.Id}");
        //    MudDialog.Close(DialogResult.Ok(created));
        //}
        //else
        //{
        //    _processing = false;
        //}
    }
}