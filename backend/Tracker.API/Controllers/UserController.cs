using MediatR;
using Microsoft.AspNetCore.Mvc;
using Tracker.Application.UseCases.Users.GetUserById;
using Tracker.API.Services;
using Microsoft.AspNetCore.Authorization;
using Tracker.Application.UseCases.Users.SearchUsersByUsername;
using Tracker.Domain.Entities;

namespace Tracker.API.Controllers;

[Route("api/users")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;
    public UserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> GetAsync([FromRoute] Guid id)
    {
        var query = new GetUserByIdQuery() { Id = id };
        var response = await _mediator.Send(query);
        return response.ToActionResult();
    }

    [HttpGet()]
    public async Task<IActionResult> GetUserByUsername([FromQuery] SearchUsersByUsernamePartQuery query)
    {
        var response = await _mediator.Send(query);
        return response.ToActionResult();
    }
}
