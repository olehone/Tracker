namespace ArchivingFunction.Domain.Options;

public class BlobOptions
{
    public const string SectionName = "BlobOptions";
    public required string ConnectionString { get; init; }

    public required string ArchiveContainerName { get; init; }
    public required TimeSpan ArchiveSasExpiration { get; init; }
}