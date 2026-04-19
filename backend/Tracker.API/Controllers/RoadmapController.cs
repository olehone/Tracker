using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tracker.API.Requests;
using Tracker.API.Services;
using Tracker.Application.UseCases.Roadmap.Get;
using Tracker.Application.UseCases.Roadmap.Save;

namespace Tracker.API.Controllers;

[Authorize]
[ApiController]
[Route("api/boards/{boardId:guid}/roadmap")]
public class RoadmapController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAsync(Guid boardId)
    {
        var result = await mediator.Send(new GetRoadmapQuery { BoardId = boardId });
        return result.ToActionResult();
    }

    [HttpPut]
    public async Task<IActionResult> SaveAsync(Guid boardId, [FromBody] SaveRoadmapRequest request)
    {
        var result = await mediator.Send(new SaveRoadmapCommand
        {
            BoardId = boardId,
            Nodes = request.Nodes.Select(n => new SaveRoadmapNodeCommand
            {
                BoardItemId = n.BoardItemId,
                X = n.X,
                Y = n.Y
            }).ToList(),
            Arrows = request.Arrows.Select(a => new SaveRoadmapArrowCommand
            {
                SourceBoardItemId = a.SourceBoardItemId,
                TargetBoardItemId = a.TargetBoardItemId,
                SourceSide = a.SourceSide,
                TargetSide = a.TargetSide
            }).ToList()
        });

        return result.ToActionResult();
    }
}
