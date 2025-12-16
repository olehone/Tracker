using MediatR;
using Microsoft.AspNetCore.Mvc;
using Tracker.Application.UseCases.Users.GetUserById;
using Tracker.API.Services;
using Microsoft.AspNetCore.Authorization;

namespace Tracker.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;
    public UserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> GetAsync([FromRoute] Guid id)
    {
        var query = new GetUserByIdQuery() { Id = id };
        var response = await _mediator.Send(query);
        return response.ToActionResult();
    }
}
