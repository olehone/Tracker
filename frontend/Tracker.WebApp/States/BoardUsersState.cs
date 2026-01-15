using Tracker.API.Requests;
using Tracker.Domain.Dtos;
using Tracker.Domain.Enums;
using Tracker.Services.Abstraction;
using Tracker.WebApp.Shared;

namespace Tracker.WebApp.States;

public sealed class BoardUsersState
{
    private readonly BoardState _boardState;
    private readonly IBoardUserService _boardUserService;
    private readonly IUserService _userService;
    private readonly IErrorNotifier _notifier;

    private readonly List<BoardUserDto> _boardUsers = [];

    public IReadOnlyList<BoardUserDto> BoardUsers => _boardUsers;
    public event Action? OnChange;

    private BoardFullDto Board => _boardState.Board!;

    public BoardUsersState(
        BoardState boardState,
        IBoardUserService boardUserService,
        IUserService userService,
        IErrorNotifier notifier)
    {
        _boardState = boardState;
        _boardUserService = boardUserService;
        _userService = userService;
        _notifier = notifier;
    }

    public async Task LoadAsync()
    {
        if (_boardState.Board is null)
        {
            return;
        }

        var result = await _boardUserService.GetUsersByBoardAsync(Board.Id);
        if (!_notifier.NotifyIfError(result))
        {
            return;
        }

        _boardUsers.Clear();
        _boardUsers.AddRange(result.Value);
    }

    public async Task<IEnumerable<UserDto>> SearchAsync(string value, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return [];
        }

        var request = new PaginatedSearchRequest
        {
            SearchQuery = value,
            AmountInPage = 5,
            Page = 1
        };

        var result = await _userService.GetUsersAsync(request);
        return result.IsSuccess
            ? result.Value.Items
            : [];
    }

    public async Task AddUserAsync(Guid userId, UserBoardRole role)
    {
        var result = await _boardUserService.AddUserToBoardAsync(Board.Id, userId, role);
        if (!_notifier.NotifyIfError(result))
        {
            return;
        }

        _boardUsers.Add(result.Value);
        Notify();
    }

    public async Task ChangeRoleAsync(BoardUserDto boardUser, UserBoardRole newRole)
    {
        var result = await _boardUserService.ChangeUserRoleAsync(Board.Id, boardUser.User.Id, newRole);
        if (!_notifier.NotifyIfError(result))
        {
            return;
        }

        boardUser.Role = newRole;
        Notify();
    }

    public async Task RemoveBoardUserAsync(BoardUserDto boardUser)
    {
        if (boardUser.Role == UserBoardRole.Owner)
        {
            return;
        }

        var result = await _boardUserService.RemoveUserFromBoardAsync(Board.Id, boardUser.User.Id);
        if (!_notifier.NotifyIfError(result))
        {
            return;
        }

        _boardUsers.Remove(boardUser);
        Notify();
    }

    public async Task TransferOwnershipAsync(BoardUserDto boardUser)
    {
        if (!Board.Permissions.CanChangeOwner)
        {
            return;
        }

        var result = await _boardUserService.ChangeUserRoleAsync(Board.Id, boardUser.User.Id, UserBoardRole.Owner);
        if (!_notifier.NotifyIfError(result))
        {
            return;
        }

        var previousOwner = _boardUsers.FirstOrDefault(u => u.Role == UserBoardRole.Owner);
        if (previousOwner is not null)
        {
            previousOwner.Role = UserBoardRole.Admin;
        }
        boardUser.Role = UserBoardRole.Owner;
        Notify();
    }



    public bool CanChangeMembers()
        => Board.Permissions.CanChangeBoard;

    public bool CanChangeMember(BoardUserDto member)
        => CanChangeMembers() && member.Role != UserBoardRole.Owner;

    public bool IsUserMember(UserDto user)
        => _boardUsers.Any(u => u.User.Id == user.Id);

    private void Notify() => OnChange?.Invoke();
}
