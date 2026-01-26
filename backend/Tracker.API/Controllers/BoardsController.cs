using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tracker.API.Requests;
using Tracker.API.Services;
using Tracker.Application.UseCases.Boards.Create;
using Tracker.Application.UseCases.Boards.Delete;
using Tracker.Application.UseCases.Boards.GetById;
using Tracker.Application.UseCases.Boards.GetForCurrentUser;
using Tracker.Application.UseCases.Boards.Update;

namespace Tracker.API.Controllers;

[Route("api/boards")]
[ApiController]
[Authorize]
public class BoardsController(IMediator mediator) : ControllerBase
{
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByIdAsync(Guid id)
    {
        var mediatorRequest = new GetBoardByIdQuery() { Id = id };
        var response = await mediator.Send(mediatorRequest);
        return response.ToActionResult();
    }

    [HttpPut("{id:guid}/settings")]
    public async Task<IActionResult> UpdateAsync(Guid id,
            [FromBody] UpdateBoardBodyRequest request)
    {
        var mediatorRequest = new UpdateBoardCommand
        {
            BoardId = id,
            Title = request.Title,
            Description = request.Description,
            Visibility = request.Visibility,
            PermissionRoles = request.PermissionRoles,
        };
        var response = await mediator.Send(mediatorRequest);
        return response.ToActionResult();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var mediatorRequest = new DeleteBoardCommand
        {
            BoardId = id,
        };
        var response = await mediator.Send(mediatorRequest);
        return response.ToActionResult();
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetForCurrentUserAsync()
    {
        var response = await mediator.Send(new GetBoardsForCurrentUserQuery());
        return response.ToActionResult();
    }
}
