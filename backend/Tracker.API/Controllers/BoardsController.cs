using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tracker.API.Requests;
using Tracker.API.Services;
using Tracker.Application.UseCases.Boards.Create;
using Tracker.Application.UseCases.Boards.GetBoardById;
using Tracker.Application.UseCases.Boards.GetBoardsByWorkspaceId;

namespace Tracker.API.Controllers;

[Route("api/boards")]
[ApiController]
[Authorize]
public class BoardsController(IMediator mediator) : ControllerBase
{
    [HttpGet("{Id:guid}")]
    public async Task<IActionResult> GetAsync([FromRoute] GetByIdRequest request)
    {
        var mediatorRequest = new GetBoardByIdQuery() { Id = request.Id };
        var response = await mediator.Send(mediatorRequest);
        return response.ToActionResult();
    }

    [HttpGet]
    public async Task<IActionResult> GetAllInWorkspaceAsync(
        [FromQuery] GetByIdRequest request)
    {
        var mediatorRequest = new GetBoardsByWorkspaceIdQuery() { WorkspaceId = request.Id };
        var response = await mediator.Send(mediatorRequest);
        return response.ToActionResult();
    }

    [HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] CreateBoardRequest request)
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
}
