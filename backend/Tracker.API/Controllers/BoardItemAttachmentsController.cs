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
    [HttpGet]
    public async Task<IActionResult> GetAllAsync(Guid boardId, Guid itemId)
    {
        var mediatorRequest = new GetItemAttachmentsCommand
        {
            BoardId = boardId,
            BoardItemId = itemId
        };
        var result = await mediator.Send(mediatorRequest);
        if (result.IsSuccess)
        {
            foreach (var attachment in result.Value)
            {
                attachment.Url = GetUrl(boardId, itemId, attachment.Id);
            }
        }
        return result.ToActionResult();
    }

    // This is for resolving URLs inside texts
    // Remove bandwidth for big files, download directly from storage
    // Stream inline files directly, browser will resolve inline content
    [HttpGet("{attachmentId:guid}", Name = "DownloadAsync")]
    [AllowAnonymous]
    public async Task<IActionResult> DownloadAsync(Guid boardId, Guid itemId, Guid attachmentId,
        [FromQuery] bool isDirect = false)
    {
        var mediatorRequest = new DownloadAttachmentCommand
        {
            BoardId = boardId,
            BoardItemId = itemId,
            AttachmentId = attachmentId,
            ForceDirect = isDirect,
        };
        var result = await mediator.Send(mediatorRequest);

        if (result.IsFailure)
        {
            return result.ToActionResult();
        }

        return Redirect(result.Value.RedirectUrl!);
        if (isDirect)
        {
            return File(result.Value.Stream!, result.Value.ContentType, result.Value.FileName, true);
        }
        else
        {
            return Redirect(result.Value.RedirectUrl!);
        }
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
        if (response.IsSuccess)
        {
            response.Value.Url = GetUrl(boardId, itemId, response.Value.Id);
        }

        return response.ToActionResult();
    }

    [HttpDelete("{attachmentId:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid boardId, Guid itemId, Guid attachmentId)
    {
        var mediatorRequest = new DeleteAttachmentCommand
        {
            BoardId = boardId,
            BoardItemId = itemId,
            AttachmentId = attachmentId,
        };
        var response = await mediator.Send(mediatorRequest);
        return response.ToActionResult();
    }

    private string GetUrl(Guid boardId, Guid itemId, Guid attachmentId)
    {
        return Url.Link( 
            nameof(DownloadAsync),
            new { boardId, itemId, attachmentId })!;
    }
}
