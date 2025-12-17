using Tracker.Domain.Entities;

namespace Tracker.Services.Abstraction;

public interface IAuthStorage
{
    Task<AuthSession?> GetAsync();
    Task SetAsync(AuthSession tokens);
    Task ClearAsync();
}
