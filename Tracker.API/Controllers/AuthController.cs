using Microsoft.AspNetCore.Mvc;
using Tracker.Domain.DTOs;
using MediatR;
using Tracker.Application.UseCases.Auth.Login;
using Tracker.Application.UseCases.Auth.Register;

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
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand request)
    {
        var response = await _mediator.Send(request);
        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserCommand request)
    {
        var response = await _mediator.Send(request);
        return Ok(response);
    }
}