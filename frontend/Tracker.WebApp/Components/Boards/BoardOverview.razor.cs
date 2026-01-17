using Microsoft.AspNetCore.Components;
using MudBlazor;
using Tracker.Domain.Dtos;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Components.Boards;

public partial class BoardOverview : IDisposable
{
    [CascadingParameter] 
    private BoardState BoardState { get; set; } = null!;
    private BoardFullDto Board => BoardState.Board!;
    private MudDropContainer<BoardItemDto> _container = null!;
    private bool _disposed;

    protected override void OnInitialized()
    {
        BoardState.Items.OnChange += OnBoardStateChanged;
        BoardState.Lists.OnChange += OnBoardStateChanged;
    }

    private void OnBoardStateChanged()
    {
        InvokeAsync(() =>
        {
            StateHasChanged();
            _container?.Refresh();
        });
    }

    private async Task ItemDropped(MudItemDropInfo<BoardItemDto> dropInfo)
    {
        if (dropInfo.Item is null)
        {
            return;
        }

        await BoardState.Items.MoveBoardItemAsync(dropInfo.Item.Id,
            dropInfo.DropzoneIdentifier,
            dropInfo.IndexInZone + 1);
    }

    private async Task CreateList(string title)
    {
        await BoardState.Lists.CreateBoardListAsync(title);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                BoardState.Items.OnChange -= OnBoardStateChanged;
                BoardState.Lists.OnChange -= OnBoardStateChanged;
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