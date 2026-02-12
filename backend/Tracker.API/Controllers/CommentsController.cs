using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tracker.API.Requests;

namespace Tracker.API.Controllers;

[Route("api/board/{boardId:guid}/items/{itemId:guid}/comments")]
[ApiController]
[Authorize]
public class CommentsController(IMediator mediator) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAsync([FromQuery] PaginatedSearchRequest request)
    {
        return BadRequest();
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync()
    {
        return BadRequest();
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