using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tracker.API.Requests;
using Tracker.API.Services;
using Tracker.Application.UseCases.Workspaces.ChangeSettings;
using Tracker.Application.UseCases.Workspaces.Create;
using Tracker.Application.UseCases.Workspaces.GetAll;
using Tracker.Application.UseCases.Workspaces.GetById;
using Tracker.Application.UseCases.Workspaces.GetForCurrentUser;
using Tracker.Application.UseCases.Workspaces.GetSettings;
using Tracker.Domain.Enums;

namespace Tracker.API.Controllers;

[Route("api/workspaces")]
[ApiController]
[Authorize]
public class WorkspaceController(IMediator mediator) : ControllerBase
{

    [HttpGet("{Id:guid}")]
    public async Task<IActionResult> GetWorkspaceByIdAsync([FromRoute] GetByIdRequest request)
    {
        var mediatorRequest = new GetWorkspaceByIdQuery()
        {
            Id = request.Id
        };
        var response = await mediator.Send(mediatorRequest);
        return response.ToActionResult();
    }

    [HttpGet("{Id:guid}/settings")]
    public async Task<IActionResult> GetWorkspaceSettingsAsync([FromRoute] GetByIdRequest request)
    {
        var mediatorRequest = new GetWorkspaceSettingsQuery()
        {
            WorkspaceId = request.Id
        };
        var response = await mediator.Send(mediatorRequest);
        return response.ToActionResult();
    }

    [HttpPut("{Id:guid}/settings")]
    public async Task<IActionResult> ChangeWorkspaceSettingsAsync([FromRoute] GetByIdRequest workspaceId,
        [FromBody] ChangeWorkspaceSettingsRequest request)
    {
        var mediatorRequest = new ChangeWorkspaceSettingsCommand()
        {
            WorkspaceId = workspaceId.Id,
            CanChangeSettings = request.CanChangeSettings,
            Visibility = request.Visibility,
            PermissionRoles = request.PermissionRoles,
        };
        var response = await mediator.Send(mediatorRequest);
        return response.ToActionResult();
    }

    [HttpGet("/all")]
    [Authorize(Roles = "Admin,Owner")]
    public async Task<IActionResult> GetAllWorkspacesAsync()
    {
        var response = await mediator.Send(new GetAllWorkspacesQuery());
        return response.ToActionResult();
    }

    [HttpGet]
    public async Task<IActionResult> GetWorkspacesForCurrentUserAsync()
    {
        var response = await mediator.Send(new GetWorkspacesForCurrentUserQuery());
        return response.ToActionResult();
    }

    [HttpPost]
    public async Task<IActionResult> CreateWorkspaceAsync([FromBody] CreateWorkspaceRequest request)
    {
        var mediatorRequest = new CreateWorkspaceCommand()
        {
            Title = request.Title,
            Description = request.Description ?? string.Empty
        };
        var response = await mediator.Send(mediatorRequest);
        return response.ToActionResult();
    }
}
