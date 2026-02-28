namespace Tracker.Domain.Options;

public class StripeOptions
{
    public const string SectionName = "StripeOptions";

    public required string SecretKey { get; init; }
    public required string WebHookSecret { get; init; }
    public required string BasicSubscriptionName { get; init; }
    public required string ProSubscriptionName { get; init; }
    public required string SuccessUrl { get; init; }
    public required string CancelUrl { get; init; }
}