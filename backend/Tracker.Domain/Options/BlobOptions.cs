namespace Tracker.Domain.Options;

public class BlobOptions
{
    public const string SectionName = "BlobOptions";
    public required string DefaultConnectionString { get; init; }

    public required string AvatarContainerName { get; init; }
    public required string[] AvatarContentTypes { get; init; }
    public required int AvatarMaxSize { get; init; }

    public required string ItemAttachmentContainerName { get; init; }
    public required int ItemAttachmentMaxSize { get; init; }
    public required TimeSpan ItemAttachmentExpiration{ get; init; }
}