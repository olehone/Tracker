using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Tracker.API.Hubs;
using Tracker.API.Hubs.Events;
using Tracker.API.Requests;
using Tracker.API.Services;
using Tracker.Application.Common.Auth;
using Tracker.Application.UseCases.BoardItems.Create;
using Tracker.Application.UseCases.BoardItems.Delete;
using Tracker.Application.UseCases.BoardItems.Move;
using Tracker.Application.UseCases.BoardItems.Update;
using Tracker.Domain.Requests.BoardItem;

namespace Tracker.API.Controllers;
[Route("api/board/{boardId:guid}/items")]
[ApiController]

[Authorize]
public class BoardItemsController(IMediator mediator,
    IHubContext<BoardHub, IClientBoardHub> hubContext,
     IUserContext userContext)
    : ControllerBase
{

    [HttpPost("{boardListId:guid}")]
    public async Task<IActionResult> CreateAsync(Guid boardId, Guid boardListId,
        [FromBody] CreateBoardItemRequest request)
    {
        var mediatorRequest = new CreateBoardItemCommand()
        {
            BoardId = boardId,
            BoardListId = boardListId,
            Title = request.Title,
            Description = request.Description
        };
        var response = await mediator.Send(mediatorRequest);
        if (response.IsSuccess)
        {
            var userId = userContext.GetUserId();
            var evt = new ItemCreatedEvent(
                UserId: userId,
                BoardId: boardId,
                Item: response.Value
            );
            await hubContext.Clients.Group($"board:{boardId}").ItemCreated(evt);
        }
        return response.ToActionResult();
    }

    [HttpPost("move/{itemId:guid}")]
    public async Task<IActionResult> MoveAsync(Guid boardId, Guid itemId, [FromBody] MoveBoardItemRequest request)
    {
        var mediatorRequest = new MoveBoardItemCommand()
        {
            BoardId = boardId,
            ToBoardListId = request.ToBoardListId,
            BoardItemId = itemId,
            Position = request.Position
        };
        var response = await mediator.Send(mediatorRequest);

        if (response.IsSuccess)
        {
            var userId = userContext.GetUserId();
            var evt = new ItemMovedEvent(
                UserId: userId,
                BoardId: boardId,
                ToListId: request.ToBoardListId,
                ItemId: itemId,
                Position: request.Position
            );
            await hubContext.Clients.Group($"board:{boardId}").ItemMoved(evt);
        }
        return response.ToActionResult();
    }


    [HttpPut("{itemId:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid boardId, Guid itemId,
        [FromBody] UpdateBoardItemRequest request)
    {
        var mediatorRequest = new UpdateBoardItemCommand
        {
            BoardId = boardId,
            BoardItemId = itemId,
            Title = request.Title,
            Description = request.Description,
            IsDone = request.IsDone
        };
        var response = await mediator.Send(mediatorRequest);
        if (response.IsSuccess)
        {
            var userId = userContext.GetUserId();
            var evt = new ItemUpdatedEvent(
                UserId: userId,
                BoardId: boardId,
                Item: response.Value
            );
            await hubContext.Clients.Group($"board:{boardId}").ItemUpdated(evt);
        }
        return response.ToActionResult();
    }

    [HttpDelete("{itemId:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid boardId, Guid itemId)
    {
        var mediatorRequest = new DeleteBoardItemCommand { BoardId = boardId, BoardItemId = itemId };
        var response = await mediator.Send(mediatorRequest);
        if (response.IsSuccess)
        {
            var userId = userContext.GetUserId();
            var evt = new ItemDeletedEvent(
                UserId: userId,
                BoardId: boardId,
                ItemId: itemId
            );
            await hubContext.Clients.Group($"board:{boardId}").ItemDeleted(evt);
        }
        return response.ToActionResult();
    }
}