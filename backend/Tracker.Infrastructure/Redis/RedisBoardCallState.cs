using Microsoft.Extensions.Options;
using StackExchange.Redis;
using Tracker.Application.Common.States;
using Tracker.Domain.Options;

namespace Tracker.Infrastructure.Redis;

internal class RedisBoardCallState(IConnectionMultiplexer redis, IOptions<RedisOptions> options)
    : IBoardCallState
{
    private readonly IDatabase _db = redis.GetDatabase();

    public async Task<Guid?> GetCallIdAsync(Guid boardId)
    {
        var id = await _db.StringGetAsync(Key(boardId));
        return id.HasValue
            ? Guid.Parse(id!)
            : null;
    }

    public async Task AddCallAsync(Guid boardId, Guid callId)
    {
        await _db.StringSetAsync(Key(boardId), callId.ToString(), Expiration);
    }

    public async Task RemoveCallAsync(Guid boardId)
    {
        await _db.KeyDeleteAsync(Key(boardId));
    }

    private TimeSpan Expiration => options.Value.CallExpiration;

    private string Key(Guid boardId)
    {
        return $"{options.Value.BoardCallKey}{boardId}";
    }
}