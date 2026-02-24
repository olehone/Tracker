namespace Tracker.Domain.Options;

public class RedisOptions
{
    public const string SectionName = "RedisOptions";

    public required string ConnectionString { get; init; }

    public required TimeSpan CallExpiration { get; init; }
    public required string CallsKey { get; init; }
    public required string ConnectionsKey { get; init; }
}