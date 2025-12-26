using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tracker.API.Requests;
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
    public async Task<IActionResult> Post([FromBody] CreateBoardListRequest request)
    {
        var mediatorRequest = new CreateBoardListCommand()
        {
            BoardId = request.BoardId,
            Title = request.Title,
            Description = request.Description
        };
        var response = await mediator.Send(mediatorRequest);
        return response.ToActionResult();
    }

    [HttpPost("move")]
    public async Task<IActionResult> Post([FromBody] MoveBoardListRequest request)
    {
        var mediatorRequest = new MoveBoardListCommand()
        {
            BoardListId = request.BoardListId,
            Position = request.Position
        };
        var response = await mediator.Send(mediatorRequest);
        return response.ToActionResult();
    }
}
