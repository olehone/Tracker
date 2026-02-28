namespace Tracker.Domain.Options;

public class StripeOptions
{
    public const string SectionName = "StripeOptions";

    public required string SecretKey { get; init; }
    public required string WebHookSecret { get; init; }
    public required string BaseSubscriptionName { get; init; }
    public required string ProSubscriptionName { get; init; }
}