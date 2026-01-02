namespace Tracker.Domain.Options;

public class ApiOptions
{
    public const string SectionName = "ApiOptions";

    public required string ApiBaseUrl { get; init; } = null!;
}
