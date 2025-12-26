using MediatR;
using Microsoft.AspNetCore.Mvc;
using Tracker.Application.UseCases.Users.GetUserById;
using Tracker.API.Services;
using Microsoft.AspNetCore.Authorization;
using Tracker.Application.UseCases.Users.SearchUsersByUsername;
using Tracker.Domain.Entities;
using Tracker.API.Requests;

namespace Tracker.API.Controllers;

[Route("api/users")]
[ApiController]
public class UserController(IMediator mediator) : ControllerBase
{

    [HttpGet("{Id:guid}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> GetAsync([FromRoute] GetByIdRequest request)
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
        }
        var response = await mediator.Send(mediatorRequest);
        return response.ToActionResult();
    }
}
