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
public class UserController(IMediator mediator) : ControllerBase
{

    [HttpGet("{Id:guid}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> GetAsync([FromRoute] GetUserByIdQuery request)
    {
        var response = await mediator.Send(request);
        return response.ToActionResult();
    }

    [HttpGet()]
    public async Task<IActionResult> GetUserByUsernameAsync(
        [FromQuery] SearchUsersByUsernamePartQuery request)
    {
        var response = await mediator.Send(request);
        return response.ToActionResult();
    }
}
