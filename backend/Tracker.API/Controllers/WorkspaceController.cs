using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tracker.API.Requests;
using Tracker.API.Services;
using Tracker.Application.UseCases.Auth.Register;
using Tracker.Application.UseCases.Workspaces.GetWorkspaceById;
using Tracker.Application.UseCases.Workspaces.GetWorkspaces;

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

    [HttpGet]
    public async Task<IActionResult> GetWorkspacesAsync()
    {
        var response = await mediator.Send(new GetWorkspacesQuery());
        return response.ToActionResult();
    }

    [HttpPost]
    public async Task<IActionResult> CreateWorkspaceAsync([FromBody] CreateWorkspaceRequest request)
    {
        var mediatorRequest = new CreateWorkspaceCommand()
        {
            Title = request.Title,
            Description = request.Description
        };
        var response = await mediator.Send(mediatorRequest);
        return response.ToActionResult();
    }
}
