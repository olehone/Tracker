using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tracker.API.Requests;
using Tracker.API.Services;
using Tracker.Application.UseCases.Boards.Create;
using Tracker.Application.UseCases.Boards.GetById;
using Tracker.Application.UseCases.Boards.Update;
using Tracker.Application.UseCases.Workspaces.Update;

namespace Tracker.API.Controllers;

[Route("api/boards")]
[ApiController]
[Authorize]
public class BoardsController(IMediator mediator) : ControllerBase
{
    [HttpGet("{Id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetBoardByIdAsync([FromRoute] GetByIdRequest request)
    {
        var mediatorRequest = new GetBoardByIdQuery() { Id = request.Id };
        var response = await mediator.Send(mediatorRequest);
        return response.ToActionResult();
    }

    [HttpPost]
    public async Task<IActionResult> CreateBoardAsync([FromBody] CreateBoardRequest request)
    {
        var mediatorRequest = new CreateBoardCommand()
        {
            WorkspaceId = request.WorkspaceId,
            Title = request.Title,
            Description = request.Description
        };
        var response = await mediator.Send(mediatorRequest);
        return response.ToActionResult();
    }

    [Authorize]
    [HttpPut("{Id:guid}/settings")]
    public async Task<IActionResult> UpdateAsync([FromRoute] GetByIdRequest boardId,
            [FromBody] UpdateBoardBodyRequest request)
    {
        var mediatorRequest = new UpdateBoardCommand
        {
            BoardId = boardId.Id,
            Title = request.Title,
            Description = request.Description,
            Visibility = request.Visibility,
            PermissionRoles = request.PermissionRoles,
        };
        var response = await mediator.Send(mediatorRequest);
        return response.ToActionResult();
    }
}
