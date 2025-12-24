using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tracker.API.Services;
using Tracker.Application.UseCases.BoardLists.Create;
using Tracker.Application.UseCases.BoardLists.Move;

namespace Tracker.API.Controllers;

[Route("api/board-lists")]
[ApiController]
[Authorize]
public class BoardListsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateBoardListCommand request)
    {
        var response = await mediator.Send(request);
        return response.ToActionResult();
    }

    [HttpPost("move")]
    public async Task<IActionResult> Post([FromBody] MoveBoardListCommand request)
    {
        var response = await mediator.Send(request);
        return response.ToActionResult();
    }
}
