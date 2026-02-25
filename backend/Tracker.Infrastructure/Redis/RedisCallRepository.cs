using System.Text.Json;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using Tracker.Application.Common.Repositories;
using Tracker.Domain.Entities;
using Tracker.Domain.Options;

namespace Tracker.Infrastructure.Redis;

// Naming this a repository hides implementation details,
// but it may be confusing because it is not part of the Unit of Work.
// Would RedisCallState and ICallState be a better choice ?
internal class RedisCallRepository(IConnectionMultiplexer redis,IOptions<RedisOptions> options) 
    : ICallRepository
{
    private readonly IDatabase _db = redis.GetDatabase();

    public async Task<Call?> GetCallByIdAsync(Guid callId)
    {
        var json = await _db.StringGetAsync(CallKey(callId));
        return json.HasValue 
            ? JsonSerializer.Deserialize<Call>(json!) 
            : null;
    }

    public async Task<Call?> GetCallByConnectionAsync(string connectionId)
    {
        var call = await _db.StringGetAsync(ConnectionKey(connectionId));
        return call.HasValue
            ? JsonSerializer.Deserialize<Call>(call!)
            : null;
    }

    public async Task SaveCallAsync(Call call)
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
        var call = await GetCallByIdAsync(callId);
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