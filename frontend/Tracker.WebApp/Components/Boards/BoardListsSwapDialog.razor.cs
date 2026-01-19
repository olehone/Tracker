using Microsoft.AspNetCore.Components;
using MudBlazor;
using Tracker.Domain.Dtos;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Components.Boards;

public partial class BoardListsSwapDialog : IDisposable
{
    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; } = null!;
    [Parameter, EditorRequired]
    public BoardState BoardState { get; set; } = null!;

    private IReadOnlyList<BoardListDto> Lists => 
        BoardState.Lists.BoardLists;
    private bool _disposed;

    protected override void OnInitialized()
    {
        BoardState.Lists.OnChange += OnStateChanged;
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

        await BoardState.Lists.MoveAsync(
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
                BoardState.Lists.OnChange -= OnStateChanged;
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