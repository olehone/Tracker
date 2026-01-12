using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Tracker.Domain.Requests;
using Tracker.Domain.Results;

namespace Tracker.Services.Abstraction;

public interface IAuthService
{
    EventCallback OnLogin { get; set; }
    EventCallback OnLogout { get; set; }
    Task<Result> LoginAsync(LoginUserRequest request);
    Task<Result> RegisterAsync(RegisterUserRequest request);
    Task<Result> LogoutAsync();
    Task<string?> GetAccessTokenAsync();
    Task<ClaimsPrincipal> GetPrincipalAsync();
}