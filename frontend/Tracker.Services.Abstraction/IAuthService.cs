using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Tracker.Domain.Requests;

namespace Tracker.Services.Abstraction;

public interface IAuthService
{
    EventCallback OnLogin { get; set; }
    EventCallback OnLogout { get; set; }
    Task LoginAsync(LoginUserRequest request);
    Task RegisterAsync(RegisterUserRequest request);
    Task LogoutAsync();
    Task <ClaimsPrincipal> GetPrincipalAsync();
    Task<string?> GetAccessTokenAsync();
}