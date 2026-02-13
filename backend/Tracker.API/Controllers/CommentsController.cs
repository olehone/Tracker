using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tracker.API.Requests;
using Tracker.API.Services;
using Tracker.Application.UseCases.ItemComments.Create;
using Tracker.Application.UseCases.ItemComments.Get;
using Tracker.Application.UseCases.ItemComments.UploadAttachment;

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

    [HttpPost("/api/comments/{commentId:guid}")]
    public async Task<IActionResult> UploadAttachmentAsync(Guid commentId,
        [FromForm] FileUploadRequest request)
    {
        await using Stream stream = request.File.OpenReadStream();
        var mediatorRequest = new UploadCommentAttachmentCommand
        {
            CommentId = commentId,
            Content = stream,
            ContentType = request.File.ContentType,
            FileName = request.File.FileName,
            ContentLength = request.File.Length,
        };
        var response = await mediator.Send(mediatorRequest);
        return response.ToActionResult();
    }

    [HttpDelete("/api/comments/{commentId:guid}/attachments/{attachmentId:guid}")]
    public async Task<IActionResult> DeleteAttachmentAsync(Guid attachmentId)
    {
        return BadRequest();
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