using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tracker.API.Requests;
using Tracker.API.Services;
using Tracker.Application.UseCases.BoardItemAttachments.Delete;
using Tracker.Application.UseCases.BoardItemAttachments.Download;
using Tracker.Application.UseCases.BoardItemAttachments.GetAll;
using Tracker.Application.UseCases.BoardItemAttachments.Upload;

namespace Tracker.API.Controllers;

[Route("api/board/{boardId:guid}/items/{itemId:guid}/attachments")]
[ApiController]
[Authorize]
public class BoardItemAttachmentsController(IMediator mediator) : ControllerBase
{
    [HttpGet("/attachments/{attachmentId:guid}", Name = "DownloadAsync")]
    public async Task<IActionResult> DownloadAsync(Guid attachmentId,
        [FromQuery] bool isDirect = false, [FromQuery] bool isRedirect = true)
    {
        var mediatorRequest = new DownloadAttachmentCommand
        {
            AttachmentId = attachmentId,
            ForceDirect = isDirect,
        };
        var result = await mediator.Send(mediatorRequest);
        if (result.IsFailure)
        {
            return result.ToActionResult();
        }
        return isRedirect
            ? Redirect(result.Value)
            : result.ToActionResult();
    }

    [HttpDelete("/attachments/{attachmentId:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid attachmentId)
    {
        var mediatorRequest = new DeleteAttachmentCommand
        {
            AttachmentId = attachmentId,
        };
        var response = await mediator.Send(mediatorRequest);
        return response.ToActionResult();
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync(Guid boardId, Guid itemId)
    {
        var mediatorRequest = new GetItemAttachmentsCommand
        {
            BoardId = boardId,
            BoardItemId = itemId
        };
        var result = await mediator.Send(mediatorRequest);
        return result.ToActionResult();
    }

    [HttpPost]
    public async Task<IActionResult> UploadAsync(Guid boardId, Guid itemId,
        [FromForm] FileUploadRequest request)
    {
        await using Stream stream = request.File.OpenReadStream();
        var mediatorRequest = new UploadAttachmentCommand
        {
            BoardId = boardId,
            BoardItemId = itemId,
            Content = stream,
            ContentType = request.File.ContentType,
            FileName = request.File.FileName,
            ContentLength = request.File.Length
        };

        var response = await mediator.Send(mediatorRequest);
        return response.ToActionResult();
    }
}