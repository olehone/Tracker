namespace Tracker.Domain.Results;

public sealed record Error(
    string Code,
    ErrorType Type,
    string Description,
    string[]? Details = null)
{
    public static readonly Error None = new(string.Empty, ErrorType.None, "No error");
    public static readonly Error Unknown = new("Unknown",
        ErrorType.Unknown,
        "An unexpected error occurred");
}