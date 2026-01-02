using Blazored.LocalStorage;
using Tracker.Domain.Dtos;
using Tracker.Services.Abstraction.Auth;

namespace Tracker.Services.Auth;

public class AuthStorage : IAuthStorage
{
    private const string Key = "auth_tokens";
    private readonly ILocalStorageService _storage;

    public AuthStorage(ILocalStorageService storage)
        => _storage = storage;

    public async Task<TokensDto?> GetAsync()
        => await _storage.GetItemAsync<TokensDto?>(Key);

    public async Task SetAsync(TokensDto session)
        => await _storage.SetItemAsync(Key, session);

    public async Task ClearAsync()
        => await _storage.RemoveItemAsync(Key);
}