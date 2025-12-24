using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tracker.API.Services;
using Tracker.Application.UseCases.BoardItems.Create;
using Tracker.Application.UseCases.BoardItems.Move;

namespace Tracker.API.Controllers;
[Route("api/board-items")]
[ApiController]
[Authorize]
public class BoardItemsController : ControllerBase
{
    private readonly IMediator _mediator;

    public BoardItemsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateBoardItemCommand request)
    {
        var response = await _mediator.Send(request);
        return response.ToActionResult();
    }

    [HttpPost("move")]
    public async Task<IActionResult> Post([FromBody] MoveBoardItemCommand request)
    {
        var response = await _mediator.Send(request);
        return response.ToActionResult();
    }
}