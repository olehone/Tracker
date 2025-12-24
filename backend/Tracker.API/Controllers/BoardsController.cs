using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tracker.API.Services;
using Tracker.Application.UseCases.Boards.Create;
using Tracker.Application.UseCases.Boards.GetBoardById;
using Tracker.Application.UseCases.Boards.GetBoardsByWorkspaceId;

namespace Tracker.API.Controllers;

[Route("api/boards")]
[ApiController]
[Authorize]
public class BoardsController(IMediator mediator) : ControllerBase
{
    [HttpGet("{Id:guid}")]
    public async Task<IActionResult> GetAsync([FromRoute] GetBoardByIdQuery request)
    {
        var response = await mediator.Send(request);
        return response.ToActionResult();
    }

    [HttpGet]
    public async Task<IActionResult> GetAllInWorkspaceAsync(
        [FromQuery] GetBoardsByWorkspaceIdQuery request)
    {
        var response = await mediator.Send(request);
        return response.ToActionResult();
    }

    [HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] CreateBoardCommand request)
    {
        var response = await mediator.Send(request);
        return response.ToActionResult();
    }
}
