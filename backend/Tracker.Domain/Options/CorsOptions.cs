namespace Tracker.Domain.Options;

public class CorsOptions
{
    public const string SectionName = "CorsOptions";
    public required string[] AllowedOrigins { get; init; }
}