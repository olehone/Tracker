using System.Security.Claims;
using Tracker.Domain.Requests;
using Tracker.Domain.Results;

namespace Tracker.Services.Abstraction;

public interface IAuthService
{
    event Action? AuthStateChanged;
    Task<Result> LoginAsync(LoginUserRequest request);
    Task<Result> RegisterAsync(RegisterUserRequest request);
    Task LogoutAsync();
    Task<string?> GetAccessTokenAsync();
    Task<ClaimsPrincipal> GetPrincipalAsync();
}