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

    [HttpPost("{boardListId:guid}")]
    public async Task<IActionResult> CreateBoardItemAsync(Guid boardListId, 
        [FromBody] CreateBoardItemRequest request)
    {
        var mediatorRequest = new CreateBoardItemCommand()
        {
            BoardListId = boardListId,
            Title = request.Title,
            Description = request.Description
        };
        var response = await mediator.Send(mediatorRequest);
        return response.ToActionResult();
    }

    [HttpPost("move")]
    public async Task<IActionResult> MoveBoardItemAsync([FromBody] MoveBoardItemCommand request)
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