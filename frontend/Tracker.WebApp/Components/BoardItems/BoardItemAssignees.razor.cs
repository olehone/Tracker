using Microsoft.AspNetCore.Components;
using MudBlazor;
using Tracker.Domain.Dtos;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Components.BoardItems;

public partial class BoardItemAssignees : IDisposable
{
    [Parameter, EditorRequired]
    public BoardState BoardState { get; set; } = null!;
    [Parameter, EditorRequired]
    public EventCallback Close { get; set; }
    [Parameter, EditorRequired]
    public BoardItemDto Item { get; set; }

    protected BoardUsersState UsersState => BoardState.UsersState;
    private string _search = string.Empty;

    private string Search
    {
        get { return _search; }
        set
        {
            _assignedUsers = null;
            _unassignedUsers = null;
            _search = value;
        }
    }
    private bool _disposed;
    private IReadOnlyList<BoardUserDto>? _assignedUsers = null;
    private IReadOnlyList<BoardUserDto>? _unassignedUsers = null;

    private IEnumerable<BoardUserDto> Users()
    {
        return UsersState.Users.Where(bu => bu.User.Username
            .Contains(_search, StringComparison.OrdinalIgnoreCase));
    }

    private IReadOnlyList<BoardUserDto> AssignedUsers
    {
        get
        {
            _assignedUsers ??= Users().Where(bu => Item.Assignees.Contains(bu.User.Id)).ToList();
            return _assignedUsers;
        }
    }

    private IReadOnlyList<BoardUserDto> UnassignedUsers
    {
        get
        {
            _unassignedUsers ??= Users().Where(bu => !Item.Assignees.Contains(bu.User.Id)).ToList();
            return _unassignedUsers;
        }
    }
    protected override void OnParametersSet()
    {
        BoardState.ItemsState.OnChange += StateHasChangedHandler;
        BoardState.UsersState.OnChange += StateHasChangedHandler;
    }

    private void StateHasChangedHandler()
    {
        _assignedUsers = null;
        _unassignedUsers = null;
        InvokeAsync(StateHasChanged);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (BoardState?.UsersState != null)
        {
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