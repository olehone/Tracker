using System.Text.Json;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using Tracker.Application.Common.States;
using Tracker.Domain.Entities;
using Tracker.Domain.Options;

namespace Tracker.Infrastructure.Redis;

internal class RedisCallState(IConnectionMultiplexer redis,IOptions<RedisOptions> options) 
    : ICallState
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

        var callTask = transaction.StringSetAsync(CallKey(call.Id), json, Expiration);
        var userTasks = call.Users
            .Select(user => transaction.StringSetAsync(ConnectionKey(user.ConnectionId), user.User.Id.ToString(), Expiration))
            .ToList();

        await transaction.ExecuteAsync();

        await callTask;
        await Task.WhenAll(userTasks);
    }

    public async Task RemoveCallAsync(Guid callId)
    {
        var call = await GetCallByIdAsync(callId);
        if (call is null)
        {
            return;
        }

        var transaction = _db.CreateTransaction();

        var callTask = transaction.KeyDeleteAsync(CallKey(call.Id));
        var userTasks = call.Users
            .Select(user => transaction.KeyDeleteAsync(ConnectionKey(user.ConnectionId)))
            .ToList();

        await transaction.ExecuteAsync();

        await callTask;
        await Task.WhenAll(userTasks);
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