using Microsoft.AspNetCore.Components;
using MudBlazor;
using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.BoardItem;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Components.BoardItems;

public partial class BoardItem
{
    [CascadingParameter]
    private BoardState BoardState { get; set; } = null!;

    [Parameter, EditorRequired]
    public BoardItemDto Item { get; set; } = null!;

    [Inject]
    private IDialogService DialogService { get; set; } = null!;

    private async Task OnIsDoneChanged(bool isDone)
    {
        await BoardState.ItemsState.UpdateAsync(
            Item.Id,
            new UpdateBoardItemRequest
            {
                Title = Item.Title,
                Description = Item.Description,
                IsDone = isDone
            });
        Item.IsDone = isDone;
        StateHasChanged();
    }

    private string ItemStyle =>
        Item.IsDone ? "text-decoration: line-through;" : string.Empty;

    private List<BoardUserDto> AssignedUsers()
    {
        return BoardState.UsersState.Users.Where(bu => Item.Assignees.Contains(bu.User.Id)).ToList();
    }

    private async Task OpenItemSettings()
    {
        var parameters = new DialogParameters
        {
            { nameof(BoardItemSettingsDialog.BoardState), BoardState },
            { nameof(BoardItemSettingsDialog.Item), Item }
        };
        var options = new DialogOptions
        {
            CloseButton = false,
            NoHeader = true,
            MaxWidth = MaxWidth.Small
        };
        var dialog = await DialogService.ShowAsync<BoardItemSettingsDialog>(
            Item.Title,
            parameters,
            options);

        await dialog.Result;
    }
}