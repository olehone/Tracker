using System.Security.Claims;
using Tracker.Domain.Dtos;
using Tracker.Domain.Requests;

namespace Tracker.Services.Abstraction;

public interface IAuthService
{
    public event Action? OnLogin;
    public event Action? OnLogout;
    Task LoginAsync(LoginUserRequest request);
    Task RegisterAsync(RegisterUserRequest request);
    Task LogoutAsync();
    Task <ClaimsPrincipal> GetPrincipalAsync();
    Task<string?> GetAccessTokenAsync();
}