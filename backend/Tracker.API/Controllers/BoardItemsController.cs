using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tracker.API.Hubs;
using Tracker.API.Requests;
using Tracker.API.Services;
using Tracker.Application.UseCases.BoardItems.Create;
using Tracker.Application.UseCases.BoardItems.Delete;
using Tracker.Application.UseCases.BoardItems.Move;
using Tracker.Application.UseCases.BoardItems.Update;
using Tracker.Domain.Requests.BoardItem;

namespace Tracker.API.Controllers;
[Route("api/board-items")]
[ApiController]
[Authorize]
public class BoardItemsController(IMediator mediator, BoardHub hub) : ControllerBase
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
    public async Task<IActionResult> MoveBoardItemAsync([FromBody] MoveBoardItemRequest request)
    {
        var mediatorRequest = new MoveBoardItemCommand()
        {
            ToBoardListId = request.ToBoardListId,
            BoardItemId = request.BoardItemId,
            Position = request.Position
        };
        var response = await mediator.Send(mediatorRequest);

        if (response.IsSuccess)
        {
            await hub.ItemMoved(request, response.Value);
        }
        return response.ToActionResult();
    }


    [HttpPut("{boardItemId:guid}")]
    public async Task<IActionResult> UpdateBoardItemAsync(Guid boardItemId,
        [FromBody] UpdateBoardItemRequest request)
    {
        var mediatorRequest = new UpdateBoardItemCommand
        {
            BoardItemId = boardItemId,
            Title = request.Title,
            Description = request.Description
        };
        var response = await mediator.Send(mediatorRequest);
        return response.ToActionResult();
    }

    [HttpDelete("{boardItemId:guid}")]
    public async Task<IActionResult> DeleteBoardItemAsync(Guid boardItemId)
    {
        var mediatorRequest = new DeleteBoardItemCommand { BoardItemId = boardItemId };
        var response = await mediator.Send(mediatorRequest);
        return response.ToActionResult();
    }
}