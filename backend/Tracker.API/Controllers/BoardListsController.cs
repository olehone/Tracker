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
public class BoardListsController : ControllerBase
{
    private readonly IMediator _mediator;

    public BoardListsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateBoardListCommand request)
    {
        var response = await _mediator.Send(request);
        return response.ToActionResult();
    }

    [HttpPost("move")]
    public async Task<IActionResult> Post([FromBody] MoveBoardListCommand request)
    {
        var response = await _mediator.Send(request);
        return response.ToActionResult();
    }
}
