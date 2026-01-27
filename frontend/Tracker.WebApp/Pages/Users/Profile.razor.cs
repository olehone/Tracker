using Microsoft.AspNetCore.Components;
using Tracker.Domain.Dtos;
using Tracker.Domain.Requests;
using Tracker.Services.Abstraction;
using Tracker.WebApp.Shared;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Pages.Users;

public partial class Profile
{
    [Parameter]
    public Guid UserId { get; set; }

    [Inject] IUserService UserService { get; set; } = null!;
    [Inject] AppState AppState { get; set; } = null!;
    [Inject] IErrorNotifier ErrorNotifier { get; set; } = null!;

    private bool IsOwnProfile => UserId == AppState.CurrentUser?.Id;
    private UserDto? User { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await LoadUser();
    }

    protected override async Task OnParametersSetAsync()
    {
        if (User == null || User.Id != UserId)
        {
            await LoadUser();
        }
    }
    private async Task LoadUser()
    {
        var result = await UserService.GetByIdAsync(UserId);
        if (result.IsFailure)
        {
            return;
        }

        User = result.Value;
        StateHasChanged();
    }

    private async Task<Paginated<WorkspaceSummaryDto>> LoadWorkspaces(
        PaginatedSearchRequest request)
    {
        var result = await UserService.GetMutualWorkspacesAsync(UserId, request);
        if (ErrorNotifier.NotifyIfError(result))
        {
            return Paginated<WorkspaceSummaryDto>.Empty();
        }
        return result.Value;
    }

    private async Task<Paginated<WorkspaceSummaryDto>> LoadAllWorkspaces(
    PaginatedSearchRequest request)
    {
        var result = await UserService.GetAllWorkspacesAsync(UserId, request);
        if (ErrorNotifier.NotifyIfError(result))
        {
            return Paginated<WorkspaceSummaryDto>.Empty();
        }
        return result.Value;
    }

}