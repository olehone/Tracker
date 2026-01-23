using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tracker.API.Requests;
using Tracker.API.Services;
using Tracker.Application.UseCases.Boards.Create;
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

    [HttpGet("{workspaceId:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByIdAsync(Guid workspaceId)
    {
        var mediatorRequest = new GetWorkspaceByIdQuery()
        {
            Id = workspaceId
        };
        var response = await mediator.Send(mediatorRequest);
        return response.ToActionResult();
    }

    [HttpPut("{id:guid}/settings")]
    public async Task<IActionResult> UpdateAsync(Guid id,
            [FromBody] UpdateWorkspaceBodyRequest request)
    {
        var mediatorRequest = new UpdateWorkspaceCommand
        {
            WorkspaceId = id,
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
    public async Task<IActionResult> GetAsync(
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
    public async Task<IActionResult> GetForCurrentUserAsync()
    {
        var response = await mediator.Send(new GetWorkspacesForCurrentUserQuery());
        return response.ToActionResult();
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateWithTitleRequest request)
    {
        var mediatorRequest = new CreateWorkspaceCommand()
        {
            Title = request.Title,
        };
        var response = await mediator.Send(mediatorRequest);
        return response.ToActionResult();
    }


    [HttpPost("{workspaceId:guid}/boards")]
    public async Task<IActionResult> CreateBoardAsync(Guid workspaceId,
        [FromBody] CreateWithTitleRequest request)
    {
        var mediatorRequest = new CreateBoardCommand()
        {
            WorkspaceId = workspaceId,
            Title = request.Title,
        };
        var response = await mediator.Send(mediatorRequest);
        return response.ToActionResult();
    }
}
