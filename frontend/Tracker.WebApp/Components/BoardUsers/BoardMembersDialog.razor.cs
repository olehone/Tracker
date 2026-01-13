using Microsoft.AspNetCore.Components;
using MudBlazor;
using Tracker.API.Requests;
using Tracker.Domain.Dtos;
using Tracker.Domain.Enums;
using Tracker.Services.Abstraction;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Components.BoardUsers;

public partial class BoardMembersDialog
{
    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; } = null!;
    private BoardFullDto Board => BoardState.CurrentBoard!;

    [Parameter, EditorRequired]
    public required UserBoardRole CurrentUserRole { get; set; }

    [Inject] private BoardState BoardState { get; set; } = null!;
    [Inject] private IBoardUserService BoardUserService { get; set; } = null!;
    [Inject] private IUserService UserService { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;

    private List<BoardUserDto> _members = [];
    private bool _isLoading = true;
    private UserDto? _selectedUser;
    private UserBoardRole _selectedRole = UserBoardRole.Observer;

    protected override async Task OnInitializedAsync()
    {
        BoardState.OnChange += StateHasChanged;
        await LoadMembers();
    }

    private async Task LoadMembers()
    {
        _isLoading = true;
        StateHasChanged();

        var result = await BoardUserService.GetUsersByBoardAsync(Board.Id);
        if (result.IsSuccess)
        {
            _members = result.Value?.ToList() ?? [];
        }
        _isLoading = false;
        StateHasChanged();
    }

    private async Task<IEnumerable<UserDto>> Search(string value, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(value))
        {
            return [];
        }
        var request = new PaginatedSearchRequest
        {
            SearchQuery = value,
            AmountInPage = 5,
            Page = 1,
        };
        var result = await UserService.GetUsersAsync(request);
        if (result.IsFailure)
        {
            return [];
        }
        return result.Value.Items;
    }

    private bool IsUserMember(UserDto user)
    {
        return _members.Any(u => u.User.Id == user.Id);
    }

    private static bool IsMemberDisabled(BoardUserDto user)
    {
        return user.Role == UserBoardRole.Owner;
    }

    private async Task AddUser()
    {
        if (_selectedUser is null)
        {
            return;
        }

        var request = new AddUserToBoardRequest
        {
            BoardId = Board.Id,
            UserId = _selectedUser.Id,
            Role = UserBoardRole.Observer
        };
        var result = await BoardUserService.AddUserToBoardAsync(request);
        if (result.IsSuccess)
        {
            _members.Add(result.Value);
        }
    }

    private async Task OnRoleChanged(BoardUserDto member, UserBoardRole newRole)
    {
        if (newRole == UserBoardRole.Owner)
        {
            return;
        }

        var request = new ChangeUserBoardRequest
        {
            BoardId = Board.Id,
            UserId = member.User.Id,
            Role = newRole
        };
        var result = await BoardUserService.ChangeUserRoleAsync(request);
        if (result.IsSuccess)
        {
            await LoadMembers();
        }
    }

    private async Task ShowTransferOwnershipDialog(BoardUserDto member)
    {
        if (!Board.Permissions.CanChangeOwner)
        {
            return;
        }

        var parameters = new DialogParameters
        {
            { "NewOwnerName", member.User.Username }
        };

        var options = new DialogOptions
        {
            CloseButton = true,
            MaxWidth = MaxWidth.Small,
            FullWidth = true
        };

        var dialog = await DialogService.ShowAsync<TransferOwnershipDialog>(
            "Transfer Ownership",
            parameters,
            options);

        var result = await dialog.Result;
        if (result is null || result.Canceled)
        {
            return;
        }

        var request = new ChangeUserBoardRequest
        {
            BoardId = Board.Id,
            UserId = member.User.Id,
            Role = UserBoardRole.Owner
        };
        var update = await BoardUserService.ChangeUserRoleAsync(request);

        if (update.IsSuccess)
        {
            await LoadMembers();
        }
    }

    private async Task OnRemoveMember(BoardUserDto member)
    {
        if (member.Role == UserBoardRole.Owner)
        {
            return;
        }
        var request = new RemoveUserFromBoardRequest
        {
            BoardId = Board.Id,
            UserId = member.User.Id
        };

        var result = await BoardUserService.RemoveUserFromBoardAsync(request);
        if (result.IsSuccess)
        {
            await LoadMembers();
        }
    }

    private bool CanChangeMembers()
    {
        return Board.Permissions.CanChangeBoard;
    }

    private bool CanChangeMember(BoardUserDto member)
    {
        return CanChangeMembers() && member.Role != UserBoardRole.Owner;
    }
}