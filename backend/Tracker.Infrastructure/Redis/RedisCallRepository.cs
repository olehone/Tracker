using System.Text.Json;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using Tracker.Application.Common.Repositories;
using Tracker.Domain.Dtos;
using Tracker.Domain.Options;

namespace Tracker.Infrastructure.Redis;

internal class RedisCallRepository(IConnectionMultiplexer redis,IOptions<RedisOptions> options) 
    : ICallRepository
{
    private readonly IDatabase _db = redis.GetDatabase();

    public async Task<CallDto?> GetCallAsync(Guid callId)
    {
        var json = await _db.StringGetAsync(CallKey(callId));
        return json.HasValue 
            ? JsonSerializer.Deserialize<CallDto>(json!) 
            : null;
    }

    public async Task SaveCallAsync(CallDto call)
    {
        var json = JsonSerializer.Serialize(call);
        var transaction = _db.CreateTransaction();

        await transaction.StringSetAsync(CallKey(call.Id), json, Expiration);
        foreach(var user in call.Users)
        {
            await transaction.StringSetAsync(ConnectionKey(user.ConnectionId), user.User.Id.ToString(), Expiration);
        }

        await transaction.ExecuteAsync();
    }

    public async Task RemoveCallAsync(Guid callId)
    {
        var call = await GetCallAsync(callId);
        if (call is null)
        {
            return;
        }

        var transaction = _db.CreateTransaction();

        await transaction.KeyDeleteAsync(CallKey(call.Id));
        foreach (var user in call.Users)
        {
            await transaction.KeyDeleteAsync(ConnectionKey(user.ConnectionId));
        }

        await transaction.ExecuteAsync();
    }

    public async Task<UserDto?> GetUserByConnectionAsync(string connectionId)
    {
        var user = await _db.StringGetAsync(ConnectionKey(connectionId));
        return user.HasValue
            ? JsonSerializer.Deserialize<UserDto>(user!)
            : null;
    }

    private TimeSpan Expiration => options.Value.CallExpiration;
    
    private string CallKey(Guid callId)
    {
        return $"{options.Value.CallsKey}{callId}";
    }

    private string ConnectionKey(string connectionId)
    {
        return $"{options.Value.ConnectionsKey}{connectionId}";
    }
}