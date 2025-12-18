using Microsoft.AspNetCore.Mvc;
using Tracker.API.Services;
using MediatR;
using Tracker.Application.UseCases.Auth.Login;
using Tracker.Application.UseCases.Auth.Register;
using Tracker.Application.UseCases.Auth.Refresh;
using Microsoft.AspNetCore.Authorization;
using Tracker.Application.UseCases.Auth.CurrentUser;

namespace Tracker.API.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterUserCommand request)
    {
        var response = await _mediator.Send(request);
        return response.ToActionResult();
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginUserCommand request)
    {
        var response = await _mediator.Send(request);
        return response.ToActionResult();
    }

    [HttpPost("refresh-token")]
    [Authorize]
    public async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshUserTokenCommand request)
    {
        var response = await _mediator.Send(request);
        return response.ToActionResult();
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUserAsync()
    {
        var response = await _mediator.Send(new GetCurrentUserQuery());
        return response.ToActionResult();
    }
}