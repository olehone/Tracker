using Microsoft.AspNetCore.Components;
using MudBlazor;
using Tracker.API.Requests;
using Tracker.Domain.Dtos;
using Tracker.Domain.Enums;
using Tracker.Services.Abstraction;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Components.BoardUsers;

public partial class AddMembers
{
        private BoardFullDto Board => BoardState.CurrentBoard!;

        [Parameter, EditorRequired]
        public required UserBoardRole CurrentUserRole { get; set; }

        [Inject] private BoardState BoardState { get; set; } = null!;
        [Inject] private IBoardUserService BoardUserService { get; set; } = null!;
        [Inject] private IUserService UserService { get; set; } = null!;

        private List<BoardUserDto> _members = [];
        private bool _isLoading = true;
        private UserDto? _selectedUser;
        private UserBoardRole _selectedRole = UserBoardRole.Observer;

        protected override async Task OnInitializedAsync()
        {
            BoardState.OnChange += StateHasChanged;
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
}