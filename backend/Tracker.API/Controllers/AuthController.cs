using Microsoft.AspNetCore.Mvc;
using Tracker.API.Services;
using MediatR;
using Tracker.Application.UseCases.Auth.Login;
using Tracker.Application.UseCases.Auth.Register;
using Tracker.Application.UseCases.Auth.Refresh;
using Tracker.API.Requests;

namespace Tracker.API.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController(IMediator mediator) : ControllerBase
{

    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterUserRequest request)
    {
        var mediatorRequest = new RegisterUserCommand()
        {
            Email = request.Email,
            Password = request.Password,
            Username = request.Username,
            FirstName = request.FirstName,
            LastName = request.LastName
        };
        var response = await mediator.Send(mediatorRequest);
        return response.ToActionResult();
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginUserRequest request)
    {
        var mediatorRequest = new LoginUserCommand()
        {
            Email = request.Email,
            Password = request.Password
        };
        var response = await mediator.Send(mediatorRequest);
        return response.ToActionResult();
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshUserTokenCommand request)
    {
        var mediatorRequest = new RefreshUserTokenCommand()
        {
            RefreshToken = request.RefreshToken
        };
        var response = await mediator.Send(mediatorRequest);
        return response.ToActionResult();
    }
}