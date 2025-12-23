using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tracker.API.Services;
using Tracker.Application.UseCases.Boards.Create;
using Tracker.Application.UseCases.Boards.GetBoardById;
using Tracker.Application.UseCases.Boards.GetBoardsByWorkspaceId;

namespace Tracker.API.Controllers;

[Route("api/workspaces/{WorkspaceId}/boards")]
[ApiController]
[Authorize]
public class BoardsController : ControllerBase
{
    private readonly IMediator _mediator;

    public BoardsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{Id:guid}")]
    public async Task<IActionResult> GetAsync([FromRoute] GetBoardByIdQuery request)
    {
        var response = await _mediator.Send(request);
        return response.ToActionResult();
    }

    [HttpGet]
    public async Task<IActionResult> GetAllInWorkspaceAsync(
        [FromRoute] GetBoardsByWorkspaceIdQuery request)
    {
        var response = await _mediator.Send(request);
        return response.ToActionResult();
    }

    [HttpPost]
    public async Task<IActionResult> PostAsync(
        [FromRoute] Guid WorkspaceId,
        [FromBody] CreateBoardCommandBody body)
    {
        var request = new CreateBoardCommand()
        {
            WorkspaceId = WorkspaceId,
            Title = body.Title,
            Description = body.Description
        };

        var response = await _mediator.Send(request);
        return response.ToActionResult();
    }
}
