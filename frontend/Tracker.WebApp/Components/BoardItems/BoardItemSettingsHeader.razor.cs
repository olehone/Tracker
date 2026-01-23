using Microsoft.AspNetCore.Components;
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
    [Parameter, EditorRequired]
    public UpdateBoardItemRequest Model { get; set; }

    [Inject] IDialogService DialogService { get; set; } = null!;

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

        await BoardState.ItemsState.DeleteAsync(Item.Id);
        MudDialog.Close(DialogResult.Ok(true));
    }

    private void Cancel() => MudDialog.Cancel();

    private string GetTitleStyle() =>
        Model.IsDone ? "text-decoration: line-through;" : string.Empty;

}