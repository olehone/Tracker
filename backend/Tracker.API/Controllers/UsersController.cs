using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tracker.API.Requests;
using Tracker.API.Services;
using Tracker.Application.UseCases.Users.GetUserById;
using Tracker.Application.UseCases.Users.GetAll;
using Tracker.Application.UseCases.Users.Current;

namespace Tracker.API.Controllers;

[Route("api/users")]
[ApiController]
[Authorize]
public class UserController(IMediator mediator) : ControllerBase
{

    [HttpGet("{Id:guid}")]
    public async Task<IActionResult> GetUserByIdAsync([FromRoute] GetByIdRequest request)
    {
        var mediatorRequest = new GetUserByIdQuery()
        {
            Id = request.Id
        };
        var response = await mediator.Send(mediatorRequest);
        return response.ToActionResult();
    }

    [HttpGet("all")]
    [Authorize(Roles = "Admin,Owner")]
    public async Task<IActionResult> GetUsersAsync(
        [FromQuery] PaginatedSearchRequest request)
    {
        var mediatorRequest = new GetUsersQuery()
        {
            SearchQuery = request.SearchQuery,
            Page = request.Page,
            AmountInPage = request.AmountInPage
        };
        var response = await mediator.Send(mediatorRequest);
        return response.ToActionResult();
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUserAsync()
    {
        var response = await mediator.Send(new GetCurrentUserQuery());
        return response.ToActionResult();
    }
}
