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
public class BoardItemsController(IMediator mediator) : ControllerBase
{

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateBoardItemCommand request)
    {
        var response = await mediator.Send(request);
        return response.ToActionResult();
    }

    [HttpPost("move")]
    public async Task<IActionResult> Post([FromBody] MoveBoardItemCommand request)
    {
        var response = await mediator.Send(request);
        return response.ToActionResult();
    }
}