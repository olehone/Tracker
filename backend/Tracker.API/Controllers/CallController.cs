using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tracker.API.Services;
using Tracker.Application.UseCases.Calls.Get;
namespace Tracker.API.Controllers;

[Route("api/call")]
[ApiController]
[Authorize]
public class CallController(IMediator mediator) : ControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync(Guid id)
    {
        var mediatorRequest = new GetCallByIdQuery { Id = id };
        var response = await mediator.Send(mediatorRequest);
        return response.ToActionResult();
    }
}
