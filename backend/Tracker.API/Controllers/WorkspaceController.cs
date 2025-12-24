using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    public async Task<IActionResult> GetAsync([FromRoute] GetWorkspaceByIdQuery request)
    {
        var response = await mediator.Send(request);
        return response.ToActionResult();
    }

    [HttpGet]
    public async Task<IActionResult> GetAsync()
    {
        var response = await mediator.Send(new GetWorkspacesQuery());
        return response.ToActionResult();
    }

    [HttpPost]
    public async Task<IActionResult> CreateWorkspaceAsync([FromBody] CreateWorkspaceCommand request)
    {
        var response = await mediator.Send(request);
        return response.ToActionResult();
    }
}
