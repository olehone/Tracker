using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tracker.API.Requests;
using Tracker.API.Services;
using Tracker.Application.UseCases.ItemComments.Create;
using Tracker.Application.UseCases.ItemComments.Get;
using Tracker.Application.UseCases.ItemComments.UploadAttachment;
using Tracker.Domain.Dtos;
using Tracker.Domain.Results;

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
        var createCommentRequest = new CreateItemCommentCommand
        {
            BoardItemId = itemId,
            Content = request.Content,
        };
        var commentResponse = await mediator.Send(createCommentRequest);
        if (commentResponse.IsFailure)
        {
            return commentResponse.ToActionResult();
        }

        // There may be better way to handle uploading several files
        // to fresh created entity, i choose this approach to not let 
        // IFormFile, and http concept, to be in application layer
        // and not to create all streams at once

        UploadCommentAttachmentCommand fileRequest;
        Result<CommentAttachmentDto> fileResponse;
        foreach (var file in request.Files)
        {
            await using Stream stream = file.OpenReadStream();
            fileRequest = new UploadCommentAttachmentCommand
            {
                CommentId = commentResponse.Value.Id,
                Content = stream,
                ContentType = file.ContentType,
                FileName = file.FileName,
                ContentLength = file.Length,
            };
            fileResponse = await mediator.Send(fileRequest);

            // Also i could add some kind of fallback for failed files
            // now user will just see comment with not all files, unlikely, but
            // not informative. Could wrap list of results, but require a lot of 
            // work on result chain 
            if (!fileResponse.IsFailure)
            {
                commentResponse.Value.Attachments.Add(fileResponse.Value);
            }
        }

        return commentResponse.ToActionResult();
    }

    [HttpPut("/api/attachments/{commentId:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid commentId,
        [FromBody] UpdateBoardItemRequest request)
    {
        return BadRequest();
    }

    [HttpDelete("/api/attachments/{commentId:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid commentId)
    {
        return BadRequest();
    }
}