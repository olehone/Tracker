namespace Tracker.Domain.Options;

public class ApiOptions
{
    public const string SectionName = "ApiOptions";

    public required string ApiBaseUrl { get; init; } = null!;
    public required string AvatarBaseUrl { get; init; } = null!;
    public required string AttachmentsBaseUrl { get; init; } = null!;
}