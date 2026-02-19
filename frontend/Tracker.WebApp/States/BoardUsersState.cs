using Tracker.Domain.Dtos;
using Tracker.Domain.Enums;
using Tracker.Domain.Requests;
using Tracker.Services.Abstraction;
using Tracker.Services.Abstraction.Board;

namespace Tracker.WebApp.States;

public sealed class BoardUsersState(
    BoardState boardState,
    IUserService userService,
    IBoardUserService boardUserService)
{
    private readonly List<BoardUserDto> _boardUsers = [];
    private List<BoardUserDto>? _sortedUsers;
    private Dictionary<Guid, BoardUserDto> _userLookup = [];

    public IReadOnlyList<BoardUserDto> Users
    {
        get
        {
            _sortedUsers ??= _boardUsers.OrderByDescending(bu => bu.Role).ToList();
            return _sortedUsers;
        }
    }

    public event Action? OnChange;

    private BoardFullDto Board => boardState.Board!;

    public void Reload()
    {
        var users = Board.BoardUsers;
        _sortedUsers = null;
        _boardUsers.Clear();
        _boardUsers.AddRange(users);
        _userLookup = _boardUsers.ToDictionary(u => u.User.Id);
    }

    public BoardUserDto? GetUser(Guid userId)
    {
        if (_userLookup is null)
        {
            _userLookup = _boardUsers.ToDictionary(u => u.User.Id);
        }
        return _userLookup.TryGetValue(userId, out var user)
            ? user
            : null;
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

        var result = await userService.GetAsync(request);
        return result.IsSuccess
            ? result.Value.Items
            : [];
    }

    public async Task AddAsync(Guid userId, BoardUserRole role)
    {
        var result = await boardUserService.AddAsync(Board.Id, userId, role);
        if (result.IsFailure)
        {
            return;
        }

        _boardUsers.Add(result.Value);
        _userLookup.Add(userId, result.Value);
        Notify();
    }

    public async Task ChangeRoleAsync(BoardUserDto boardUser, BoardUserRole newRole)
    {
        boardUser.Role = newRole;
        Notify();

        var result = await boardUserService.ChangeRoleAsync(Board.Id, boardUser.User.Id, newRole);
        if (result.IsFailure)
        {
            await boardState.ReloadAsync();
        }
    }

    public async Task RemoveAsync(BoardUserDto boardUser)
    {
        if (boardUser.Role == BoardUserRole.Owner)
        {
            return;
        }
        _boardUsers.Remove(boardUser);
        Notify();

        var result = await boardUserService.RemoveAsync(Board.Id, boardUser.User.Id);
        if (result.IsFailure)
        {
            await boardState.ReloadAsync();
        }
    }

    public async Task TransferOwnershipAsync(BoardUserDto boardUser)
    {
        if (!Board.Permissions.CanChangeOwner)
        {
            return;
        }

        var previousOwner = _boardUsers.FirstOrDefault(u => u.Role == BoardUserRole.Owner);
        if (previousOwner is not null)
        {
            previousOwner.Role = BoardUserRole.Admin;
        }
        boardUser.Role = BoardUserRole.Owner;
        Notify();

        var result = await boardUserService.ChangeRoleAsync(Board.Id, boardUser.User.Id, BoardUserRole.Owner);
        if (result.IsFailure)
        {
            await boardState.ReloadAsync();
        }
    }

    public bool CanChangeMembers()
        => Board.Permissions.CanChangeBoard;

    public bool IsUserMember(UserDto user)
        => _boardUsers.Any(u => u.User.Id == user.Id);

    private void Notify()
    {
        _sortedUsers = null;
        OnChange?.Invoke();
    }
}
