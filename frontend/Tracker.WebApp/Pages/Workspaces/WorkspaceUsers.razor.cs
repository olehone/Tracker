using Microsoft.AspNetCore.Components;
using MudBlazor;
using Tracker.API.Requests;
using Tracker.Domain.Dtos;
using Tracker.Domain.Enums;
using Tracker.Services.Abstraction;
using Tracker.WebApp.Shared;

namespace Tracker.WebApp.Pages.Workspaces;

public partial class WorkspaceUsers
{
    [Parameter]
    public Guid WorkspaceId { get; set; }

    [Inject] IWorkspaceService WorkspaceService { get; set; } = null!;
    [Inject] IWorkspaceUserService WorkspaceUserService { get; set; } = null!;
    [Inject] IUserService UserService { get; set; } = null!;
    [Inject] IResultNotifier Notifier { get; set; } = null!;

    private WorkspaceFullDto? Workspace { get; set; }
    private List<WorkspaceUserDto> WorkspaceUsersList { get; set; } = new();
    private bool isLoading = true;

    private UserDto? _selectedUser;
    private UserWorkspaceRole _selectedRole = UserWorkspaceRole.Member;
    private List<UserDto> _availableUsers = new();

    protected override async Task OnInitializedAsync()
    {
        await LoadData();
    }

    protected override async Task OnParametersSetAsync()
    {
        if (Workspace == null || Workspace.Id != WorkspaceId)
        {
            await LoadData();
        }
    }

    private async Task LoadData()
    {
        isLoading = true;
        await LoadWorkspace();
        await LoadWorkspaceUsers();
        isLoading = false;
    }

    private async Task LoadWorkspace()
    {
        var result = await WorkspaceService.GetByIdAsync(WorkspaceId);
        if (result.IsSuccess)
        {
            Workspace = result.Value;
        }
    }

    private async Task LoadWorkspaceUsers()
    {
        var result = await WorkspaceUserService.GetUsersByWorkspaceAsync(WorkspaceId);
        if (result.IsSuccess)
        {
            WorkspaceUsersList = result.Value;
        }
    }

    private string PageTitle()
    {
        return Workspace?.Title ?? "Workspace";
    }

    public async Task<IEnumerable<UserDto>> SearchAsync(string searchTerm, CancellationToken ct)
    {

        if (string.IsNullOrWhiteSpace(searchTerm) || searchTerm.Length < 2)
        {
            return [];
        }

        var request = new PaginatedSearchRequest
        {
            SearchQuery = searchTerm,
            AmountInPage = 5,
            Page = 1
        };
        var result = await UserService.GetAsync(request);

        return result.IsSuccess
            ? result.Value.Items
            : [];
    }

    private bool IsUserMember(UserDto user)
    {
        return WorkspaceUsersList.Any(wu => wu.User.Id == user.Id);
    }

    private async Task AddUserAsync()
    {
        if (_selectedUser is null)
            return;

        var result = await WorkspaceUserService.AddUserToWorkspaceAsync(
            WorkspaceId,
            _selectedUser.Id,
            _selectedRole);

        if (result.IsSuccess)
        {
            _selectedUser = null;
            _selectedRole = UserWorkspaceRole.Member;
            await LoadWorkspaceUsers();
        }
    }

    private async Task ChangeUserRoleAsync(WorkspaceUserDto workspaceUser, UserWorkspaceRole newRole)
    {
        if (workspaceUser.Role == newRole)
            return;

        var result = await WorkspaceUserService.ChangeUserRoleAsync(
            WorkspaceId,
            workspaceUser.User.Id,
            newRole);

        if (result.IsSuccess)
        {
            await LoadWorkspaceUsers();
        }
    }

    private async Task RemoveUserAsync(WorkspaceUserDto workspaceUser)
    {
        var result = await WorkspaceUserService.RemoveUserFromWorkspaceAsync(
            WorkspaceId,
            workspaceUser.User.Id);

        if (result.IsSuccess)
        {
            await LoadWorkspaceUsers();
        }
    }

    private static Color GetRoleColor(UserWorkspaceRole role)
    {
        return role switch
        {
            UserWorkspaceRole.Owner => Color.Warning,
            UserWorkspaceRole.Admin => Color.Error,
            UserWorkspaceRole.Member => Color.Primary,
            _ => Color.Default
        };
    }

    private static string GetRoleIcon(UserWorkspaceRole role)
    {
        return role switch
        {
            UserWorkspaceRole.Owner => Icons.Material.Filled.Star,
            UserWorkspaceRole.Admin => Icons.Material.Filled.AdminPanelSettings,
            UserWorkspaceRole.Member => Icons.Material.Filled.Person,
            _ => Icons.Material.Filled.PersonOff
        };
    }

    private bool CanChangeRole(WorkspaceUserDto workspaceUser)
    {
        return Workspace!.Permissions.CanChangeWorkspace &&
            workspaceUser.Role != UserWorkspaceRole.Owner;
    }
}