using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tracker.API.Requests;
using Tracker.API.Services;
using Tracker.Application.UseCases.Attachments.Delete;
using Tracker.Application.UseCases.Attachments.Download;
using Tracker.Application.UseCases.Attachments.Upload;
using Tracker.Domain.Enums;
using Tracker.Domain.Results;

namespace Tracker.API.Controllers;

[Route("/api/attachments/{attachmentId:guid}")]
[ApiController]
[Authorize]
public class AttachmentsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> DownloadAsync(Guid attachmentId,
        [FromQuery] AttachmentType type)
    {
        var mediatorRequest = new DownloadAttachmentCommand
        {
            AttachmentId = attachmentId,
            Type = type,
            ForceDirect = true,
        };
        var result = await mediator.Send(mediatorRequest);
        if (result.IsFailure)
        {
            return result.ToActionResult();
        }
        return File(result.Value.Stream!, result.Value.ContentType, result.Value.FileName, true);
    }

    [HttpGet("url")]
    public async Task<IActionResult> GetUrlAsync(Guid attachmentId,
        [FromQuery] AttachmentType type, [FromQuery] bool isRedirect = true)
    {
        var mediatorRequest = new DownloadAttachmentCommand
        {
            AttachmentId = attachmentId,
            Type = type,
            ForceDirect = false,
        };
        var result = await mediator.Send(mediatorRequest);
        if (result.IsFailure)
        {
            return result.ToActionResult();
        }
        var url = result.Value.RedirectUrl!;
        return isRedirect
            ? Redirect(url)
            : Result.SuccessOf(url).ToActionResult();
    }

    [HttpPost]
    public async Task<IActionResult> UploadAsync(Guid parentId,
        [FromForm] FileUploadRequest request, [FromQuery] AttachmentType type)
    {
        await using Stream stream = request.File.OpenReadStream();
        var mediatorRequest = new UploadAttachmentCommand
        {
            ParentId = parentId,
            Type = type,
            Content = stream,
            ContentType = request.File.ContentType,
            FileName = request.File.FileName,
            ContentLength = request.File.Length
        };

        var response = await mediator.Send(mediatorRequest);
        return response.ToActionResult();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteAsync(Guid attachmentId,
        [FromQuery] AttachmentType type)
    {
        var mediatorRequest = new DeleteAttachmentCommand
        {
            AttachmentId = attachmentId,
            Type = type
        };
        var response = await mediator.Send(mediatorRequest);
        return response.ToActionResult();
    }
}