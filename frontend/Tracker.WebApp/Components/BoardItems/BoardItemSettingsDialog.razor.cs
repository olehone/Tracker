using Microsoft.AspNetCore.Components;
using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.BoardItem;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Components.BoardItems;

public partial class BoardItemSettingsDialog : IDisposable
{
    private bool _openAssign;
    private bool _disposed;
    private string _description = string.Empty;
    private bool _isEditingDescription = false;

    [Parameter]
    public BoardState BoardState { get; set; } = null!;

    [Parameter]
    public BoardItemDto Item { get; set; } = null!;

    private bool IsItemExists =>
        BoardState.ItemsState.BoardItems.Any(i => i.Id == Item.Id);

    private void ToggleAssign()
    {
        _openAssign = !_openAssign;
    }

    protected override void OnInitialized()
    {
        BoardState.ItemsState.OnChange += OnChange;
        _description = Item.Description;
    }

    private void OnChange()
    {
        _description = Item.Description;
        StateHasChanged();
    }

    protected override void OnParametersSet()
    {
        if (!_isEditingDescription && _description != Item.Description)
        {
            _description = Item.Description;
        }
    }

    private void DescriptionFocused()
    {
        _isEditingDescription = true;
    }

    private async Task DescriptionBlurred()
    {
        _isEditingDescription = false;

        if (_description == Item.Description)
        {
            return;
        }

        var request = new UpdateBoardItemRequest { Description = _description };
        await BoardState.ItemsState.UpdateAsync(Item.Id, request);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            BoardState.ItemsState.OnChange -= StateHasChanged;
        }

        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}