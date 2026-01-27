using Microsoft.AspNetCore.Components;
using MudBlazor;
using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.BoardItem;
using Tracker.Services.Abstraction.Auth;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Components.Items;

public partial class ItemFull
{
    private static readonly DialogOptions DialogOptions = new()
    {
        CloseButton = false,
        NoHeader = true,
        MaxWidth = MaxWidth.Small
    };

    [CascadingParameter]
    private BoardState BoardState { get; set; } = null!;

    [Parameter, EditorRequired]
    public BoardItemDto Item { get; set; } = null!;
    [Parameter]
    public bool OneLine { get; set; } = false;

    [Inject] IDialogService DialogService { get; set; } = null!;

    private Task OnIsDoneChanged(bool isDone)
    {
        var request = new UpdateBoardItemRequest
        {
            IsDone = isDone
        };

        return BoardState.ItemsState.UpdateAsync(Item.Id, request);
    }

    private string ItemStyle =>
        Item.IsDone ? "text-decoration: line-through;" : string.Empty;

    private IEnumerable<BoardUserDto> AssignedUsers()
    {
        return Item.Assignees.Count != 0
            ? BoardState.UsersState.Users.Where(bu => Item.Assignees.Contains(bu.User.Id))
            : [];
    }

    private bool IsOwn()
    {
        if (BoardState.IsUnauthenticated)
        {
            return false;
        }
        return Item.Assignees.Contains(BoardState.CurrentUserId);
    }

    private string GetUsersKey()
    {
        return string.Join("-", Item.Assignees);
    }
    private int GetElevation()
    {
        return IsOwn() ? 3 : 0;
    }

    private async Task OpenItemSettings()
    {
        var parameters = new DialogParameters
        {
            { nameof(ItemSettingsDialog.BoardState), BoardState },
            { nameof(ItemSettingsDialog.Item), Item }
        };

        var dialog = await DialogService.ShowAsync<ItemSettingsDialog>(
            Item.Title,
            parameters,
            DialogOptions);

        await dialog.Result;
    }
}