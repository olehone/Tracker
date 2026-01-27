using Microsoft.AspNetCore.Components;
using MudBlazor;
using Tracker.Domain.Dtos;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Components.Boards;

public partial class ListsSwapDialog : IDisposable
{
    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; } = null!;
    [Parameter, EditorRequired]
    public BoardState BoardState { get; set; } = null!;

    private IReadOnlyList<BoardListDto> Lists => 
        BoardState.ListsState.BoardLists;
    private bool _disposed;

    protected override void OnInitialized()
    {
        BoardState.ListsState.OnChange += OnStateChanged;
    }

    private void OnStateChanged()
    {
        InvokeAsync(StateHasChanged);
    }

    private async Task SwapList(MudItemDropInfo<BoardListDto> dropInfo)
    {
        if (dropInfo.Item is null)
        {
            return;
        }

        await BoardState.ListsState.MoveAsync(
            dropInfo.Item.Id,
            dropInfo.IndexInZone + 1
        );
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                BoardState.ListsState.OnChange -= OnStateChanged;
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