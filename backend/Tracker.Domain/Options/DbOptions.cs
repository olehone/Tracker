namespace Tracker.Domain.Options;

public class DbOptions
{
    public const string SectionName = "DbOptions";
    public required string DefaultConnectionString { get; init; }
}