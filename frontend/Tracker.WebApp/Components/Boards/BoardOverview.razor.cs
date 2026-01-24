using Microsoft.AspNetCore.Components;
using MudBlazor;
using Tracker.Domain.Dtos;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Components.Boards;

public partial class BoardOverview : IDisposable
{
    [CascadingParameter]
    private BoardState BoardState { get; set; } = null!;

    [Inject] AppState AppState { get; set; } = null!;
    private BoardFullDto Board => BoardState.Board!;
    private MudDropContainer<BoardItemDto> _container = null!;
    private bool _disposed;

    protected override void OnInitialized()
    {
        BoardState.ItemsState.OnChange += OnBoardStateChanged;
        BoardState.ListsState.OnChange += OnBoardStateChanged;
    }

    private void OnBoardStateChanged()
    {
        InvokeAsync(() =>
        {
            StateHasChanged();
            _container?.Refresh();
        });
    }

    private bool ItemDisabled(BoardItemDto item)
    {
        if (AppState.CurrentUser is null)
        {
            return true;
        }
        if (Board.Permissions.CanChangeItem)
        {
            return false;
        }
        if (item.Assignees.Contains(AppState.CurrentUser.Id))
        {
            return false;
        }
        return true;
    }

    private async Task ItemDropped(MudItemDropInfo<BoardItemDto> dropInfo)
    {
        if (dropInfo.Item is null)
        {
            return;
        }

        await BoardState.ItemsState.MoveAsync(dropInfo.Item.Id,
            dropInfo.DropzoneIdentifier,
            dropInfo.IndexInZone + 1);
    }

    private async Task CreateList(string title)
    {
        await BoardState.ListsState.CreateAsync(title);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                BoardState.ItemsState.OnChange -= OnBoardStateChanged;
                BoardState.ListsState.OnChange -= OnBoardStateChanged;
            }
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}