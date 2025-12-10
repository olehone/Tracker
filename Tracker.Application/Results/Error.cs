namespace Tracker.Application.Results;

public sealed record Error(
    string Code,
    ErrorType Type,
    string? Description = null)
{
    public static readonly Error None = new(string.Empty, ErrorType.None);
    public static readonly Error Unknown = new("Unknown error occurred", ErrorType.Unknown);
}
