using Tracker.Domain.Dtos;
using Tracker.Domain.Requests;

namespace Tracker.Services.Abstraction;

public interface IAuthService
{
    Task LoginAsync(LoginUserRequest request);
    Task RegisterAsync(RegisterUserRequest request);
    Task LogoutAsync();
    Task<UserDto?> GetCurrentUserAsync();
    Task<string?> GetAccessTokenAsync();
}