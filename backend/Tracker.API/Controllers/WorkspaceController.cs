using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tracker.API.Requests;
using Tracker.API.Services;
using Tracker.Application.UseCases.Workspaces.Create;
using Tracker.Application.UseCases.Workspaces.GetAll;
using Tracker.Application.UseCases.Workspaces.GetById;
using Tracker.Application.UseCases.Workspaces.GetForCurrentUser;
using Tracker.Application.UseCases.Workspaces.Update;

namespace Tracker.API.Controllers;

[Route("api/workspaces")]
[ApiController]
[Authorize]
public class WorkspaceController(IMediator mediator) : ControllerBase
{

    [HttpGet("{Id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetWorkspaceByIdAsync([FromRoute] GetByIdRequest request)
    {
        var mediatorRequest = new GetWorkspaceByIdQuery()
        {
            Id = request.Id
        };
        var response = await mediator.Send(mediatorRequest);
        return response.ToActionResult();
    }

    [HttpPut("{Id:guid}/settings")]
    public async Task<IActionResult> UpdateAsync([FromRoute] GetByIdRequest workspaceId,
            [FromBody] UpdateWorkspaceBodyRequest request)
    {
        var mediatorRequest = new UpdateWorkspaceCommand
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

    [HttpGet("all")]
    [Authorize(Roles = "Admin,Owner")]
    public async Task<IActionResult> GetWorkspacesAsync(
        [FromQuery] PaginatedSearchRequest request)
    {
        var mediatorRequest = new GetWorkspacesQuery()
        {
            SearchQuery = request.SearchQuery,
            Page = request.Page,
            AmountInPage = request.AmountInPage
        };
        var response = await mediator.Send(mediatorRequest);
        return response.ToActionResult();
    }

    [HttpGet("my")]
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
