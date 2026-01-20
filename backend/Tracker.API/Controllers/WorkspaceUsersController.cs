using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tracker.API.Requests;
using Tracker.API.Services;
using Tracker.Application.UseCases.WorkspaceUsers.GetByBoardId;
using Tracker.Application.UseCases.WorkspaceUsers.Add;
using Tracker.Application.UseCases.WorkspaceUsers.Change;
using Tracker.Application.UseCases.WorkspaceUsers.Remove;

namespace Tracker.API.Controllers;

[Route("api/workspaces/{workspaceId:guid}/users")]
[ApiController]
[Authorize]
public class WorkspaceUsersController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetUsersByWorkspaceAsync(Guid workspaceId)
    {
        var mediatorRequest = new GetUsersByWorkspaceIdQuery { WorkspaceId = workspaceId };
        var response = await mediator.Send(mediatorRequest);
        return response.ToActionResult();
    }

    [HttpPost("{userId:guid}")]
    public async Task<IActionResult> AddUserToWorkspaceAsync(Guid workspaceId, Guid userId,
        [FromBody] WorkspaceUserRoleRequest request)
    {
        var mediatorRequest = new AddUserToWorkspaceCommand
        {
            WorkspaceId= workspaceId,
            UserId = userId,
            Role = request.Role
        };
        var response = await mediator.Send(mediatorRequest);
        return response.ToActionResult();
    }

    [HttpPut("{userId:guid}")]
    public async Task<IActionResult> ChangeUserRoleAsync(Guid workspaceId, Guid userId,
        [FromBody] WorkspaceUserRoleRequest request)
    {
        var mediatorRequest = new ChangeWorkspaceUserRoleCommand
        {
            WorkspaceId = workspaceId,
            UserId = userId,
            Role = request.Role
        };
        var response = await mediator.Send(mediatorRequest);
        return response.ToActionResult();
    }

    [HttpDelete("{userId:guid}")]
    public async Task<IActionResult> RemoveUserFromWorkspaceAsync(Guid workspaceId, Guid userId)
    {
        var mediatorRequest = new RemoveUserFromWorkspaceCommand
        {
            WorkspaceId = workspaceId,
            UserId = userId,
        };
        var response = await mediator.Send(mediatorRequest);
        return response.ToActionResult();
    }
}
