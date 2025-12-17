using Blazored.LocalStorage;
using Tracker.Domain.Entities;
using Tracker.Services.Abstraction;

namespace Tracker.Services;

public class AuthStorage : IAuthStorage
{
    private const string Key = "auth_tokens";
    private readonly ILocalStorageService _storage;

    public AuthStorage(ILocalStorageService storage)
        => _storage = storage;

    public async Task<AuthSession?> GetAsync()
        => await _storage.GetItemAsync<AuthSession?>(Key);

    public async Task SetAsync(AuthSession tokens)
        => await _storage.SetItemAsync(Key, tokens);

    public async Task ClearAsync()
        => await _storage.RemoveItemAsync(Key);
  
}
