using Tracker.Domain.Dtos;

namespace Tracker.Services.Abstraction.Auth;

public interface IAuthStorage
{
    Task<TokensDto?> GetAsync();
    Task SetAsync(TokensDto session);
    Task ClearAsync();
}
