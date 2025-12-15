using Microsoft.AspNetCore.Mvc;
using Tracker.Domain.DTOs;
using Tracker.Domain.Results;
using Tracker.API.Services;
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
}