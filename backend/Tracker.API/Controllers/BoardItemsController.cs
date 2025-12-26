using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tracker.API.Requests;
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
    public async Task<IActionResult> Post([FromBody] CreateBoardItemRequest request)
    {
        var mediatorRequest = new CreateBoardItemCommand()
        {
            BoardListId = request.BoardListId,
            Title = request.Title,
            Description = request.Description
        };
        var response = await mediator.Send(mediatorRequest);
        return response.ToActionResult();
    }

    [HttpPost("move")]
    public async Task<IActionResult> Post([FromBody] MoveBoardItemCommand request)
    {
        var mediatorRequest = new MoveBoardItemCommand()
        {
            ToBoardListId = request.ToBoardListId,
            BoardItemId = request.BoardItemId,
            Position = request.Position
        };
        var response = await mediator.Send(mediatorRequest);
        return response.ToActionResult();
    }
}