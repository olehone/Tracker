using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Tracker.API.Hubs;
using Tracker.API.Hubs.Events;
using Tracker.API.Requests;
using Tracker.API.Services;
using Tracker.Application.Common.Auth;
using Tracker.Application.UseCases.BoardItemAssignees.Add;
using Tracker.Application.UseCases.BoardItemAssignees.Remove;
using Tracker.Application.UseCases.BoardItems.Create;
using Tracker.Application.UseCases.BoardItems.Delete;
using Tracker.Application.UseCases.BoardItems.GetAttachments;
using Tracker.Application.UseCases.BoardItems.Move;
using Tracker.Application.UseCases.BoardItems.Update;

namespace Tracker.API.Controllers;

[Route("api/board/{boardId:guid}/items")]
[ApiController]
[Authorize]
public class BoardItemsController(IMediator mediator,
    IHubContext<BoardHub, IClientBoardHub> hubContext,
     IUserContext userContext)
    : ControllerBase
{
    [HttpGet("{itemId:guid}/attachments")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAttachmentsAsync(Guid boardId,
        Guid itemId)
    {
        var mediatorRequest = new GetItemAttachmentsCommand
        {
            BoardId = boardId,
            BoardItemId = itemId
        };
        var result = await mediator.Send(mediatorRequest);
        return result.ToActionResult();
    }

    [HttpPost("{boardListId:guid}")]
    public async Task<IActionResult> CreateAsync(Guid boardId, Guid boardListId,
        [FromBody] CreateWithTitleRequest request)
    {
        var mediatorRequest = new CreateBoardItemCommand()
        {
            BoardId = boardId,
            BoardListId = boardListId,
            Title = request.Title,
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


    [HttpPatch("{itemId:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid boardId, Guid itemId,
        [FromBody] UpdateBoardItemRequest request)
    {
        var mediatorRequest = new UpdateBoardItemCommand
        {
            BoardId = boardId,
            BoardItemId = itemId,
            Title = request.Title,
            Description = request.Description,
            IsDone = request.IsDone,
            DueDate = request.DueDate,
            ClearDueDate = request.ClearDueDate,
            Importance = request.Importance,
        };
        var response = await mediator.Send(mediatorRequest);
        if (response.IsSuccess)
        {
            var userId = userContext.GetUserId();
            var evt = new ItemUpdatedEvent(
                UserId: userId,
                BoardId: boardId,
                ItemId: itemId,
                ChangedFields: request
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

    [HttpPost("{itemId:guid}/assign/{userId:guid}")]
    public async Task<IActionResult> AssignAsync(Guid boardId, Guid itemId, Guid userId)
    {
        var mediatorRequest = new AddAssigneeToItemCommand
        {
            BoardId = boardId,
            BoardItemId = itemId,
            UserId = userId,
        };
        var response = await mediator.Send(mediatorRequest);

        if (response.IsSuccess)
        {
            var currentUserId = userContext.GetUserId();
            var changedFields = new UpdateBoardItemRequest
            {
                Assignees = response.Value
            };
            var evt = new ItemUpdatedEvent(
                UserId: currentUserId,
                BoardId: boardId, ItemId: itemId,
                ChangedFields: changedFields
            );
            await hubContext.Clients.Group($"board:{boardId}").ItemUpdated(evt);
        }
        return response.ToActionResult();
    }

    [HttpDelete("{itemId:guid}/assign/{userId:guid}")]
    public async Task<IActionResult> UnassignAsync(Guid boardId, Guid itemId, Guid userId)
    {
        var mediatorRequest = new RemoveAssigneeFromItemCommand
        {
            BoardId = boardId,
            BoardItemId = itemId,
            UserId = userId,
        };
        var response = await mediator.Send(mediatorRequest);

        if (response.IsSuccess)
        {
            var currentUserId = userContext.GetUserId();
            var changedFields = new UpdateBoardItemRequest
            {
                Assignees = response.Value
            };
            var evt = new ItemUpdatedEvent(
                UserId: currentUserId,
                BoardId: boardId,
                ItemId: itemId,
                ChangedFields: changedFields
            );
            await hubContext.Clients.Group($"board:{boardId}").ItemUpdated(evt);
        }
        return response.ToActionResult();
    }
}