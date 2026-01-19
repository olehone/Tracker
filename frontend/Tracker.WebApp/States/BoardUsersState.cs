using Tracker.API.Requests;
using Tracker.Domain.Dtos;
using Tracker.Domain.Enums;
using Tracker.Services.Abstraction;

namespace Tracker.WebApp.States;

public sealed class BoardUsersState(
    BoardState boardState,
    IUserService userService,
    IBoardUserService boardUserService)
{
    private readonly List<BoardUserDto> _boardUsers = [];
    private List<BoardUserDto>? _sortedUsers;
    private readonly List<Guid> _recentActiveUserIds = [];
    private Dictionary<Guid, BoardUserDto> _userLookup = [];

    public IReadOnlyList<BoardUserDto> RecentActiveUsers(int take = 5)
    {
        return _recentActiveUserIds
            .Select(GetUser)
            .Where(u => u != null)
            .Cast<BoardUserDto>()
            .Take(take)
            .ToList();
    }

    public BoardUserDto? GetUser(Guid userId)
    {
        return _userLookup != null && _userLookup.TryGetValue(userId, out var user)
            ? user
            : null;
    }

    public void MarkActivity(Guid userId)
    {
        _recentActiveUserIds.Remove(userId);
        _recentActiveUserIds.Insert(0, userId);
        Cleanup();
        Notify();
    }

    private void Cleanup()
    {
        var max = 100;
        if (_recentActiveUserIds.Count > max)
        {
            _recentActiveUserIds.RemoveRange(max, _recentActiveUserIds.Count - max);
        }
    }

    public IReadOnlyList<BoardUserDto> BoardUsers
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

    public async Task AddAsync(Guid userId, UserBoardRole role)
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

    public async Task ChangeRoleAsync(BoardUserDto boardUser, UserBoardRole newRole)
    {
        var result = await boardUserService.ChangeRoleAsync(Board.Id, boardUser.User.Id, newRole);
        if (result.IsFailure)
        {
            return;
        }

        boardUser.Role = newRole;
        Notify();
    }

    public async Task RemoveAsync(BoardUserDto boardUser)
    {
        if (boardUser.Role == UserBoardRole.Owner)
        {
            return;
        }

        var result = await boardUserService.RemoveAsync(Board.Id, boardUser.User.Id);
        if (result.IsFailure)
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

        var result = await boardUserService.ChangeRoleAsync(Board.Id, boardUser.User.Id, UserBoardRole.Owner);
        if (result.IsFailure)
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

    private void Notify()
    {
        _sortedUsers = null;
        OnChange?.Invoke();
    }
}
