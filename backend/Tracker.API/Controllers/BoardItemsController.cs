using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Tracker.API.Hubs;
using Tracker.API.Hubs.Events;
using Tracker.API.Requests;
using Tracker.API.Services;
using Tracker.Application.Common.Auth;
using Tracker.Application.UseCases.BoardItems.Create;
using Tracker.Application.UseCases.BoardItems.Delete;
using Tracker.Application.UseCases.BoardItems.Move;
using Tracker.Application.UseCases.BoardItems.Update;
using Tracker.Domain.Requests.BoardItem;

namespace Tracker.API.Controllers;
[Route("api/board/{boardId:guid}/items")]
[ApiController]

[Authorize]
public class BoardItemsController(IMediator mediator,
    IHubContext<BoardHub, IClientBoardHub> hubContext,
     IUserContext userContext) : ControllerBase
{

    [HttpPost("{boardListId:guid}")]
    public async Task<IActionResult> CreateBoardItemAsync(Guid boardId, Guid boardListId,
        [FromBody] CreateBoardItemRequest request)
    {
        var mediatorRequest = new CreateBoardItemCommand()
        {
            BoardId = boardId,
            BoardListId = boardListId,
            Title = request.Title,
            Description = request.Description
        };
        var response = await mediator.Send(mediatorRequest);
        return response.ToActionResult();
    }

    [HttpPost("move/{itemId:guid}")]
    public async Task<IActionResult> MoveBoardItemAsync(Guid boardId, Guid itemId, [FromBody] MoveBoardItemRequest request)
    {
        var mediatorRequest = new MoveBoardItemCommand()
        {
            BoardId = boardId,
            ToBoardListId = request.ToBoardListId,
            BoardItemId = itemId,
            Position = request.Position
        };
        var response = await mediator.Send(mediatorRequest);

        if (response.IsSuccess)
        {
            var userId = userContext.GetUserId();
            var evn = new ItemMovedEvent(
                BoardId: boardId,
                ToBoardListId: request.ToBoardListId,
                BoardItemId: itemId,
                Position: request.Position,
                UserId: userId
            );
            await hubContext.Clients
                .Group($"board:{boardId}")
                .ItemMoved(evn);
        }
        return response.ToActionResult();
    }


    [HttpPut("{itemId:guid}")]
    public async Task<IActionResult> UpdateBoardItemAsync(Guid boardId, Guid itemId,
        [FromBody] UpdateBoardItemRequest request)
    {
        var mediatorRequest = new UpdateBoardItemCommand
        {
            BoardId = boardId,
            BoardItemId = itemId,
            Title = request.Title,
            Description = request.Description
        };
        var response = await mediator.Send(mediatorRequest);
        return response.ToActionResult();
    }

    [HttpDelete("{itemId:guid}")]
    public async Task<IActionResult> DeleteBoardItemAsync(Guid boardId, Guid itemId)
    {
        var mediatorRequest = new DeleteBoardItemCommand { BoardId = boardId, BoardItemId = itemId };
        var response = await mediator.Send(mediatorRequest);
        return response.ToActionResult();
    }
}