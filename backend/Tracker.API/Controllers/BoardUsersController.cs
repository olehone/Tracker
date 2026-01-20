using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tracker.API.Requests;
using Tracker.API.Services;
using Tracker.Application.UseCases.BoardUsers.Get;
using Tracker.Application.UseCases.BoardUsers.Add;
using Tracker.Application.UseCases.BoardUsers.Change;
using Tracker.Application.UseCases.BoardUsers.Remove;

namespace Tracker.API.Controllers;

[Route("api/boards/{boardId:guid}/users")]
[ApiController]
[Authorize]
public class BoardsUsersController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetByBoardAsync(Guid boardId)
    {
        var mediatorRequest = new GetUsersByBoardIdQuery { BoardId = boardId };
        var response = await mediator.Send(mediatorRequest);
        return response.ToActionResult();
    }

    [HttpPost("{userId:guid}")]
    public async Task<IActionResult> AddAsync(Guid boardId, Guid userId,
        [FromBody] BoardUserRoleRequest request)
    {
        var mediatorRequest = new AddUserToBoardCommand
        {
            BoardId = boardId,
            UserId = userId,
            Role = request.Role
        };
        var response = await mediator.Send(mediatorRequest);
        return response.ToActionResult();
    }

    [HttpPut("{userId:guid}")]
    public async Task<IActionResult> ChangeRoleAsync(Guid boardId, Guid userId,
        [FromBody] BoardUserRoleRequest request)
    {
        var mediatorRequest = new ChangeBoardUserRoleCommand
        {
            BoardId = boardId,
            UserId = userId,
            Role = request.Role
        };
        var response = await mediator.Send(mediatorRequest);
        return response.ToActionResult();
    }

    [HttpDelete("{userId:guid}")]
    public async Task<IActionResult> RemoveAsync(Guid boardId, Guid userId)
    {
        var mediatorRequest = new RemoveUserFromBoardCommand
        {
            BoardId = boardId,
            UserId = userId,
        };
        var response = await mediator.Send(mediatorRequest);
        return response.ToActionResult();
    }
}
