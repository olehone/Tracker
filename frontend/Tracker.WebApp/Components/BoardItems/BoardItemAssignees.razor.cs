using Microsoft.AspNetCore.Components;
using MudBlazor;
using Tracker.Domain.Dtos;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Components.BoardItems;

public partial class BoardItemAssignees : IDisposable
{
    private bool _disposed;

    [Parameter, EditorRequired]
    public BoardState BoardState { get; set; } = null!;

    [Parameter, EditorRequired]
    public EventCallback Close { get; set; }

    [Parameter, EditorRequired]
    public BoardItemDto Item { get; set; } = null!;

    protected BoardUsersState UsersState => BoardState.UsersState;

    private string Search { get; set; } = string.Empty;

    private IEnumerable<BoardUserDto> FilteredUsers()
    {
        if (string.IsNullOrWhiteSpace(Search))
        {
            return UsersState.Users;
        }

        return UsersState.Users.Where(bu =>
            bu.User.Username.Contains(Search, StringComparison.OrdinalIgnoreCase));
    }

    private IEnumerable<UserDto> AssignedUsers =>
        FilteredUsers().Where(bu => Item.Assignees.Contains(bu.User.Id)).Select(bu => bu.User);

    private IEnumerable<UserDto> UnassignedUsers =>
        FilteredUsers().Where(bu => !Item.Assignees.Contains(bu.User.Id)).Select(bu => bu.User);

    protected override void OnInitialized()
    {
        BoardState.ItemsState.OnChange += StateHasChangedHandler;
        BoardState.UsersState.OnChange += StateHasChangedHandler;
    }

    private void StateHasChangedHandler()
    {
        InvokeAsync(StateHasChanged);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            BoardState.ItemsState.OnChange -= StateHasChangedHandler;
            BoardState.UsersState.OnChange -= StateHasChangedHandler;
        }

        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}