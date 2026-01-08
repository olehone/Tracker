using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tracker.API.Requests;
using Tracker.API.Services;
using Tracker.Application.UseCases.Workspaces.Update;
using Tracker.Application.UseCases.Workspaces.Create;
using Tracker.Application.UseCases.Workspaces.GetAll;
using Tracker.Application.UseCases.Workspaces.GetById;
using Tracker.Application.UseCases.Workspaces.GetForCurrentUser;

namespace Tracker.API.Controllers;

[Route("api/workspaces")]
[ApiController]
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

    [Authorize]
    [HttpPut("{Id:guid}/settings")]
    public async Task<IActionResult> UpdateAsync([FromRoute] GetByIdRequest workspaceId,
            [FromBody] UpdateWorkspaceBodyRequest request)
    {
        var mediatorRequest = new UpdateWorkspaceCommand()
        {
            WorkspaceId = workspaceId.Id,
            Title = request.Title,
            Description = request.Description,
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

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetWorkspacesForCurrentUserAsync()
    {
        var response = await mediator.Send(new GetWorkspacesForCurrentUserQuery());
        return response.ToActionResult();
    }

    [Authorize]
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
