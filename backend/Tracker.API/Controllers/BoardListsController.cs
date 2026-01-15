using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tracker.API.Requests;
using Tracker.API.Services;
using Tracker.Application.UseCases.BoardLists.Create;
using Tracker.Application.UseCases.BoardLists.Delete;
using Tracker.Application.UseCases.BoardLists.Move;
using Tracker.Application.UseCases.BoardLists.Update;
using Tracker.Domain.Requests.BoardList;

namespace Tracker.API.Controllers;

[Route("api/board-lists")]
[ApiController]
[Authorize]
public class BoardListsController(IMediator mediator) : ControllerBase
{
    [HttpPost("{boardId:guid}")]
    public async Task<IActionResult> CreateBoardListAsync(Guid boardId,
        [FromBody] CreateBoardListRequest request)
    {
        var mediatorRequest = new CreateBoardListCommand
        {
            BoardId = boardId,
            Title = request.Title,
            Description = request.Description
        };
        var response = await mediator.Send(mediatorRequest);
        return response.ToActionResult();
    }

    [HttpPost("{boardListId:guid}/move")]
    public async Task<IActionResult> MoveBoardListAsync(Guid boardListId, 
        [FromBody] MoveBoardListRequest request)
    {
        var mediatorRequest = new MoveBoardListCommand
        {
            BoardListId = boardListId,
            Position = request.Position
        };
        var response = await mediator.Send(mediatorRequest);
        return response.ToActionResult();
    }

    [HttpPut("{boardListId:guid}")]
    public async Task<IActionResult> UpdateBoardListAsync(Guid boardListId,
        [FromBody] UpdateBoardListRequest request)
    {
        var mediatorRequest = new UpdateBoardListCommand
        {
            BoardListId = boardListId,
            Title = request.Title,
            Description = request.Description
        };
        var response = await mediator.Send(mediatorRequest);
        return response.ToActionResult();
    }

    [HttpDelete("{boardListId:guid}")]
    public async Task<IActionResult> DeleteBoardListAsync(Guid boardListId)
    {
        var mediatorRequest = new DeleteBoardListCommand { BoardListId = boardListId };
        var response = await mediator.Send(mediatorRequest);
        return response.ToActionResult();
    }
}
