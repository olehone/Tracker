using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tracker.API.Requests;
using Tracker.API.Services;
using Tracker.Application.UseCases.ItemComments.Create;
using Tracker.Application.UseCases.ItemComments.Get;

namespace Tracker.API.Controllers;

[Route("api/items/{itemId:guid}/comments")]
[ApiController]
[Authorize]
public class CommentsController(IMediator mediator) : ControllerBase
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
        return response.ToActionResult();
    }

    [HttpPut("/api/attachments/{commentId:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid commentId,
        [FromBody] UpdateBoardItemRequest request)
    {
        return BadRequest();
    }

    [HttpDelete("/api/comments/{commentId:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid commentId)
    {
        return BadRequest();
    }
}