using Tracker.Application.UseCases.Users;
using Microsoft.AspNetCore.Mvc;
using Tracker.Domain.DTOs;

namespace Tracker.API.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly RegisterUser _registerUser;
    private readonly LoginUser _loginUser;

    public AuthController(RegisterUser registerUser, LoginUser loginUser)
    {
        _registerUser = registerUser;
        _loginUser = loginUser;
    }
    [HttpPost("register")]
    public async Task<UserDto> Register([FromBody] RegisterUser.RegisterRequest request)
    {
        return await _registerUser.Handle(request);
    }

    [HttpPost("login")]
    public async Task<string> Login([FromBody] LoginUser.LoginRequest request)
    {
        return await _loginUser.Handle(request);
    }
}