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

[Route("api/boards-users")]
[ApiController]
[Authorize]
public class BoardsUsersController(IMediator mediator) : ControllerBase
{
    [HttpGet("{Id:guid}")]
    public async Task<IActionResult> GetUsersByBoardAsync([FromRoute] GetByIdRequest request)
    {
        var mediatorRequest = new SearchToAddByBoardIdQuery { BoardId = request.Id };
        var response = await mediator.Send(mediatorRequest);
        return response.ToActionResult();
    }

    [HttpPost]
    public async Task<IActionResult> AddUserToBoardAsync([FromBody] AddUserToBoardRequest request)
    {
        var mediatorRequest = new AddUserToBoardCommand
        {
            BoardId = request.BoardId,
            UserId = request.UserId,
            Role = request.Role
        };
        var response = await mediator.Send(mediatorRequest);
        return response.ToActionResult();
    }

    [HttpPut]
    public async Task<IActionResult> ChangeUserRoleAsync([FromBody] ChangeUserBoardRequest request)
    {
        var mediatorRequest = new ChangeUserRoleCommand
        {
            BoardId = request.BoardId,
            UserId = request.UserId,
            Role = request.Role
        };
        var response = await mediator.Send(mediatorRequest);
        return response.ToActionResult();
    }

    [HttpDelete]
    public async Task<IActionResult> RemoveUserFromBoardAsync(
        [FromBody] RemoveUserFromBoardRequest request)
    {
        var mediatorRequest = new RemoveUserFromBoardCommand
        {
            BoardId = request.BoardId,
            UserId = request.UserId,
        };
        var response = await mediator.Send(mediatorRequest);
        return response.ToActionResult();
    }
}
