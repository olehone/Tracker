namespace Tracker.Domain.Results;

public sealed record Error(
    string Code,
    ErrorType Type,
    string? Description = null,
    string[]? Details = null)
{
    public static readonly Error None = new(string.Empty, ErrorType.None);
    public static readonly Error Unknown = new("Unknown error occurred", ErrorType.Unknown);
    public static Error Validation(params string[] messages)
        => new("Validation", ErrorType.Validation, "Validation failed", messages);

}
