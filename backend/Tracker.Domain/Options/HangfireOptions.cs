namespace Tracker.Domain.Options;

public class HangfireOptions
{
    public const string SectionName = "HangfireOptions";

    public required string BoardArchivingCron { get; init; }
}