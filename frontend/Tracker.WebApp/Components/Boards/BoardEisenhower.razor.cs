using Microsoft.AspNetCore.Components;
using Tracker.Domain.Dtos;
using Tracker.Domain.Enums;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Components.Boards;

public partial class BoardEisenhower : IDisposable
{
    [CascadingParameter]
    private BoardState BoardState { get; set; } = null!;

    [Inject] AppState AppState { get; set; } = null!;

    private bool _disposed;
    private List<BoardItemDto> UrgentImportant = [];
    private List<BoardItemDto> UrgentUnimportant = [];
    private List<BoardItemDto> UnurgentImportant = [];
    private List<BoardItemDto> UnurgentUnimportant = [];

    protected override void OnInitialized()
    {
        BoardState.ItemsState.OnChange += OnBoardStateChanged;
        ReloadItems();
    }

    private void ReloadItems()
    {
        if (AppState.CurrentUser is null)
        {
            return;
        }

        var items = BoardState.ItemsState.BoardItems
            .Where(bi => bi.Assignees.Contains(AppState.CurrentUser.Id))
            .Where(bi => !bi.IsDone);

        UrgentImportant = items
            .Where(item => IsUrgent(item) && IsImportant(item))
            .ToList();

        UrgentUnimportant = items
            .Where(item => IsUrgent(item) && !IsImportant(item))
            .ToList();

        UnurgentImportant = items
            .Where(item => !IsUrgent(item) && IsImportant(item))
            .ToList();

        UnurgentUnimportant = items
            .Where(item => !IsUrgent(item) && !IsImportant(item))
            .ToList();
    }

    private static bool IsUrgent(BoardItemDto item)
    {
        return item.DueDate.HasValue;
    }

    private static bool IsImportant(BoardItemDto item)
    {
        return item.Importance > BoardItemImportance.Medium;
    }

    private void OnBoardStateChanged()
    {
        ReloadItems();
        StateHasChanged();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                BoardState.ItemsState.OnChange -= OnBoardStateChanged;
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