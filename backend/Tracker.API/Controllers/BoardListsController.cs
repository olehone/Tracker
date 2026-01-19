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

[Route("api/board/{boardId:guid}/lists")]
[ApiController]
[Authorize]
public class BoardListsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateAsync(Guid boardId,
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
    public async Task<IActionResult> MoveAsync(Guid boardId, Guid boardListId,
        [FromBody] MoveBoardListRequest request)
    {
        var mediatorRequest = new MoveBoardListCommand
        {
            BoardId = boardId,
            BoardListId = boardListId,
            Position = request.Position
        };
        var response = await mediator.Send(mediatorRequest);
        return response.ToActionResult();
    }

    [HttpPut("{boardListId:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid boardId, Guid boardListId,
        [FromBody] UpdateBoardListRequest request)
    {
        var mediatorRequest = new UpdateBoardListCommand
        {
            BoardId = boardId,
            BoardListId = boardListId,
            Title = request.Title,
            Description = request.Description
        };
        var response = await mediator.Send(mediatorRequest);
        return response.ToActionResult();
    }

    [HttpDelete("{boardListId:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid boardId, Guid boardListId)
    {
        var mediatorRequest = new DeleteBoardListCommand
        {
            BoardId = boardId,
            BoardListId = boardListId
        };
        var response = await mediator.Send(mediatorRequest);
        return response.ToActionResult();
    }
}
