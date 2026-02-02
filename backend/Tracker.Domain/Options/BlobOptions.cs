namespace Tracker.Domain.Options;

public class BlobOptions
{
    public const string SectionName = "BlobOptions";
    public required string DefaultConnectionString { get; init; }
}