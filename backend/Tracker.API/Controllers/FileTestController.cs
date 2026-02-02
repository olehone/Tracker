using MediatR;
using Microsoft.AspNetCore.Mvc;
using Tracker.API.Requests;
using Tracker.API.Services;
using Tracker.Application.Common.Services;
using Tracker.Domain.Results;

namespace Tracker.API.Controllers;

[Route("api/test")]
[ApiController]
public class FileTestController(IMediator mediator, IStorageService storageService) : ControllerBase
{
    [HttpPost("ping")]
    public IActionResult Ping(IFormFile file)
    {
        return Ok("Hit the method!");
    }

    [HttpPost("avatar")]
    public async Task<IActionResult> UploadPleaseAsync([FromForm] FileUploadRequest request)
    {

        await using Stream stream = request.File.OpenReadStream();
        Guid fileId = await storageService.UploadAsync(stream, request.File.ContentType);
        return Result.SuccessOf(fileId).ToActionResult();
    }

    [HttpGet("{fileId:guid}")]
    public async Task<IResult> GetAsync(Guid fileId)
    {
        FileResponse fileResponse = await storageService.DownloadAsync(fileId);
        return Results.File(fileResponse.Stream, fileResponse.ContentType);
    }

    [HttpDelete("{fileId:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid fileId)
    {
        await storageService.DeleteAsync(fileId);
        return Result.Success().ToActionResult();
    }
}
