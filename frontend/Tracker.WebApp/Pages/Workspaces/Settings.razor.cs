using FluentValidation;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.Workspace;
using Tracker.Domain.ValueObjects;
using Tracker.Services.Abstraction;
using Tracker.WebApp.Shared;

namespace Tracker.WebApp.Pages.Workspaces;

public partial class Settings
{
    [Parameter]
    public Guid WorkspaceId { get; set; }

    [Inject] IWorkspaceService WorkspaceService { get; set; } = null!;
    [Inject] IResultNotifier Notifier { get; set; } = null!;

    private WorkspaceFullDto? Workspace { get; set; }
    private UpdateWorkspaceRequest? model;
    private MudForm? _form;
    private bool isLoading = true;
    private bool isSubmitting = false;

    protected override async Task OnInitializedAsync()
    {
        await LoadWorkspace();
    }

    protected override async Task OnParametersSetAsync()
    {
        if (Workspace == null || Workspace.Id != WorkspaceId)
        {
            await LoadWorkspace();
        }
    }
    private async Task LoadWorkspace()
    {
        isLoading = true;
        try
        {
            var result = await WorkspaceService.GetByIdAsync(WorkspaceId);
            if (result.IsFailure)
            {
                return;
            }

            Workspace = result.Value;
            model = new UpdateWorkspaceRequest
            {
                Title = Workspace.Title,
                Description = Workspace.Description,
                Visibility = Workspace.Visibility,
                PermissionRoles = new WorkspacePermissionRoles
                {
                    MinCreateBoardRole = Workspace.PermissionRoles.MinCreateBoardRole,
                    MinChangeBoardRole = Workspace.PermissionRoles.MinChangeBoardRole,
                }
            };
            StateHasChanged();
        }
        finally
        {
            isLoading = false;
        }
    }

    private async Task Submit()
    {
        if (_form is null || model is null)
        {
            return;
        }

        await _form.Validate();
        if (!_form.IsValid)
        {
            return;
        }

        isSubmitting = true;

        var result = await WorkspaceService.UpdateAsync(WorkspaceId, model);
        Notifier.Notify(result);
        isSubmitting = false;
    }
    private string PageTitle()
    {
        return Workspace?.Title ?? "Workspace";
    }

    private bool IsDisabled()
    {
        if (Workspace is null)
        {
            return false;
        }
        return !Workspace.Permissions.CanChangeWorkspace;
    }
}