using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Tracker.API.Hubs;
using Tracker.API.Hubs.Events;
using Tracker.API.Hubs.Events.Comments;
using Tracker.API.Hubs.Interfaces;
using Tracker.API.Requests;
using Tracker.API.Services;
using Tracker.Application.Common.Auth;
using Tracker.Application.UseCases.ItemComments.Create;
using Tracker.Application.UseCases.ItemComments.Delete;
using Tracker.Application.UseCases.ItemComments.Get;
using Tracker.Application.UseCases.ItemComments.Update;
using Tracker.Domain.Entities;

namespace Tracker.API.Controllers;

[Route("api/items/{itemId:guid}/comments")]
[ApiController]
[Authorize]
public class CommentsController(IMediator mediator,
    IHubContext<ItemHub, IClientItemHub> hubContext,
     IUserContext userContext) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAsync(Guid itemId,
        [FromQuery] CursorTimeRequest request)
    {
        var mediatorRequest = new LoadCommentsQuery
        {
            ItemId = itemId,
            Before = request.Before,
            Take = request.Amount
        };
        var response = await mediator.Send(mediatorRequest);

        return response.ToActionResult();
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(Guid itemId,
        CreateCommentRequest request)
    {
        var mediatorRequest = new CreateItemCommentCommand
        {
            BoardItemId = itemId,
            Content = request.Content,
        };
        var response = await mediator.Send(mediatorRequest);
        if (response.IsSuccess)
        {
            var userId = userContext.GetUserId();
            var evt = new CommentCreatedEvent(
                UserId: userId,
                ItemId: itemId,
                Comment: response.Value);
            await hubContext.Clients.Group($"item:{itemId}").CommentCreated(evt);
        }
        return response.ToActionResult();
    }

    [HttpPut("{commentId:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid commentId, Guid itemId,
        [FromBody] UpdateItemCommentRequest request)
    {
        var mediatorRequest = new UpdateItemCommentCommand
        {
            CommentId = commentId,
            Content = request.Content,
        };
        var response = await mediator.Send(mediatorRequest);
        if (response.IsSuccess)
        {
            var userId = userContext.GetUserId();
            var evt = new CommentUpdatedEvent(
                UserId: userId,
                ItemId: itemId,
                Comment: response.Value);
            await hubContext.Clients.Group($"item:{itemId}").CommentUpdated(evt);
        }
        return response.ToActionResult();
    }

    [HttpDelete("{commentId:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid commentId, Guid itemId)
    {
        var mediatorRequest = new DeleteItemCommentCommand
        {
            CommentId = commentId
        };
        var response = await mediator.Send(mediatorRequest);
        if (response.IsSuccess)
        {
            var userId = userContext.GetUserId();
            var evt = new CommentDeletedEvent(
                UserId: userId,
                ItemId: itemId,
                CommentId: commentId
            );
            await hubContext.Clients.Group($"item:{itemId}").CommentDeleted(evt);
        }
        return response.ToActionResult();
    }
}