using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tracker.API.Requests;
using Tracker.API.Services;
using Tracker.Application.UseCases.BoardItems.Create;
using Tracker.Application.UseCases.BoardItems.Delete;
using Tracker.Application.UseCases.BoardItems.Move;
using Tracker.Application.UseCases.BoardItems.Update;
using Tracker.Application.UseCases.BoardLists.Delete;
using Tracker.Application.UseCases.BoardLists.Update;
using Tracker.Domain.Requests.BoardItem;
using Tracker.Domain.Requests.BoardList;

namespace Tracker.API.Controllers;
[Route("api/board-items")]
[ApiController]
[Authorize]
public class BoardItemsController(IMediator mediator) : ControllerBase
{

    [HttpPost("{boardListId:guid}")]
    public async Task<IActionResult> CreateAsync(Guid boardListId,
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
    public async Task<IActionResult> MoveAsync([FromBody] MoveBoardItemCommand request)
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


    [HttpPut("{boardItemId:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid boardItemId,
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
    public async Task<IActionResult> DeleteAsync(Guid boardItemId)
    {
        var mediatorRequest = new DeleteBoardItemCommand { BoardItemId = boardItemId };
        var response = await mediator.Send(mediatorRequest);
        return response.ToActionResult();
    }
}