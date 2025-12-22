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
public class WorkspaceController : ControllerBase
{
    private readonly IMediator _mediator;

    public WorkspaceController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetAsync([FromRoute] Guid id)
    {
        var query = new GetWorkspaceByIdQuery() { Id = id };
        var response = await _mediator.Send(query);
        return response.ToActionResult();
    }

    [HttpGet]
    public async Task<IActionResult> GetAsync()
    {
        var response = await _mediator.Send(new GetWorkspacesQuery());
        return response.ToActionResult();
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateWorkspaceAsync([FromBody] CreateWorkspaceCommand request)
    {
        var response = await _mediator.Send(request);
        return response.ToActionResult();
    }
}
