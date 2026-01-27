using Microsoft.AspNetCore.Components;
using Tracker.Domain.Dtos;
using Tracker.Domain.Enums;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Components.Boards;

public partial class BoardEisenhower
{
    [Inject] AppState AppState { get; set; } = null!;

    private List<BoardItemDto> UrgentImportant = [];
    private List<BoardItemDto> UrgentUnimportant = [];
    private List<BoardItemDto> UnurgentImportant = [];
    private List<BoardItemDto> UnurgentUnimportant = [];

    protected override void OnInitialized()
    {
        base.OnInitialized();
        BoardState.ItemsState.OnChange += StateHasChangedHandler;
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

    protected override void StateHasChangedHandler()
    {
        ReloadItems();
        base.StateHasChangedHandler();
    }

    protected override void InsideDispose()
    {
        base.InsideDispose();
        BoardState.ItemsState.OnChange -= StateHasChangedHandler;
    }
}