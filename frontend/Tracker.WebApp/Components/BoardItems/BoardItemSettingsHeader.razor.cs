using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.BoardItem;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Components.BoardItems;

public partial class BoardItemSettingsHeader
{
    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; } = null!;
    [CascadingParameter]
    public BoardState BoardState { get; set; } = null!;

    [Parameter, EditorRequired]
    public BoardItemDto Item { get; set; }

    [Inject] IDialogService DialogService { get; set; } = null!;


    private string? _title;
    private bool _isDone;
    private bool _isEditingTitle = false;

    private bool Disabled =>
        !BoardState.Board.Permissions.CanChangeItem;

    protected override void OnInitialized()
    {
        _title = Item.Title;
        _isDone = Item.IsDone;
    }

    protected override void OnParametersSet()
    {
        if (!_isEditingTitle && _title != Item.Title)
        {
            _title = Item.Title;
        }

        _isDone = Item.IsDone;
    }

    private async Task Delete()
    {
        bool? result = await DialogService.ShowMessageBox(
            "Warning",
            "Deleting can not be undone!",
            yesText: "Delete!",
            cancelText: "Cancel");

        if (result != true)
        {
            return;
        }

        MudDialog.Close(DialogResult.Ok(true));
        await BoardState.ItemsState.DeleteAsync(Item.Id);
    }

    private async Task ChangeIsDone(bool isDone)
    {
        var request = new UpdateBoardItemRequest { IsDone = isDone };
        await BoardState.ItemsState.UpdateAsync(Item.Id, request);
    }

    private void TitleFocused()
    {
        _isEditingTitle = true;
    }

    private async Task TitleBlurred(FocusEventArgs args)
    {
        _isEditingTitle = false;

        if (string.IsNullOrWhiteSpace(_title) || _title == Item.Title)
        {
            return;
        }

        var request = new UpdateBoardItemRequest { Title = _title };
        await BoardState.ItemsState.UpdateAsync(Item.Id, request);
    }

    private void Cancel() => MudDialog.Cancel();

    private string GetTitleStyle() =>
        Item.IsDone ? "text-decoration: line-through;" : string.Empty;

}