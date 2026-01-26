using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Tracker.API.Hubs;
using Tracker.API.Hubs.Events;
using Tracker.API.Requests;
using Tracker.API.Services;
using Tracker.Application.Common.Auth;
using Tracker.Application.UseCases.BoardLists.Create;
using Tracker.Application.UseCases.BoardLists.Delete;
using Tracker.Application.UseCases.BoardLists.Move;
using Tracker.Application.UseCases.BoardLists.Update;

namespace Tracker.API.Controllers;

[Route("api/board/{boardId:guid}/lists")]
[ApiController]
[Authorize]
public class BoardListsController(IMediator mediator,
    IHubContext<BoardHub, IClientBoardHub> hubContext,
     IUserContext userContext)
    : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateAsync(Guid boardId,
        [FromBody] CreateWithTitleRequest request)
    {
        var mediatorRequest = new CreateBoardListCommand
        {
            BoardId = boardId,
            Title = request.Title,
        };
        var response = await mediator.Send(mediatorRequest);

        if (response.IsSuccess)
        {
            var userId = userContext.GetUserId();
            var evt = new ListCreatedEvent(
                UserId: userId,
                BoardId: boardId,
                List: response.Value
            );
            await hubContext.Clients.Group($"board:{boardId}").ListCreated(evt);
        }
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

        if (response.IsSuccess)
        {
            var userId = userContext.GetUserId();
            var evt = new ListMovedEvent(
                UserId: userId,
                BoardId: boardId,
                ListId: boardListId,
                Position: request.Position
            );
            await hubContext.Clients.Group($"board:{boardId}").ListMoved(evt);
        }
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

        if (response.IsSuccess)
        {
            var userId = userContext.GetUserId();
            var evt = new ListUpdatedEvent(
                UserId: userId,
                BoardId: boardId,
                List: response.Value
            );
            await hubContext.Clients.Group($"board:{boardId}").ListUpdated(evt);
        }
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

        if (response.IsSuccess)
        {
            var userId = userContext.GetUserId();
            var evt = new ListDeletedEvent(
                UserId: userId,
                BoardId: boardId,
                ListId: boardListId
            );
            await hubContext.Clients.Group($"board:{boardId}").ListDeleted(evt);
        }
        return response.ToActionResult();
    }
}
