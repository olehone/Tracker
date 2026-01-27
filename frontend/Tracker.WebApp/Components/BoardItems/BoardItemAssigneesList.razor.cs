using Microsoft.AspNetCore.Components;
using MudBlazor;
using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.BoardItem;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Components.BoardItems;

public partial class BoardItemAssigneesList
{
    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; } = null!;
    [CascadingParameter]
    public BoardState BoardState { get; set; } = null!;

    [Parameter, EditorRequired]
    public BoardItemDto Item { get; set; }
    [Parameter, EditorRequired]
    public bool OpenAssign { get; set; }
    [Parameter, EditorRequired]
    public EventCallback ToggleAssign { get; set; }
    [Parameter]
    public bool Disabled { get; set; } = true;

    private IEnumerable<UserDto> AssignedUsers()
    {
        return Item.Assignees.Count != 0
            ? BoardState.UsersState.Users.Where(bu => Item.Assignees.Contains(bu.User.Id)).Select(bu => bu.User)
            : [];
    }
}