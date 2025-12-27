using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tracker.API.Requests;
using Tracker.API.Services;
using Tracker.Application.UseCases.Users.GetUserById;
using Tracker.Application.UseCases.Users.SearchUsersByUsername;
using Tracker.Application.UseCases.Users.Current;
using Tracker.Domain.Entities;

namespace Tracker.API.Controllers;

[Route("api/users")]
[ApiController]
public class UserController(IMediator mediator) : ControllerBase
{

    [HttpGet("{Id:guid}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> GetUserByIdAsync([FromRoute] GetByIdRequest request)
    {
        var mediatorRequest = new GetUserByIdQuery()
        {
            Id = request.Id
        };
        var response = await mediator.Send(mediatorRequest);
        return response.ToActionResult();
    }

    [HttpGet()]
    public async Task<IActionResult> GetUserByUsernameAsync(
        [FromQuery] SearchByFieldPartRequest request)
    {
        var mediatorRequest = new SearchUsersByUsernamePartQuery()
        {
            Username = request.SearchQuery,
            Page = request.Page,
            AmountInPage = request.AmountInPage
        };
        var response = await mediator.Send(mediatorRequest);
        return response.ToActionResult();
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUserAsync()
    {
        var response = await mediator.Send(new GetCurrentUserQuery());
        return response.ToActionResult();
    }
}
