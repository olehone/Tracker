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

    public static Error Validation(params string[] messages)
    {
        return new("Validation", ErrorType.Validation, "Validation failed", messages);
    }

    public static Error NotFound(string entityName, string propertyName = "id")
    {
        return new($"{entityName}.NotFound",
            ErrorType.NotFound,
            $"{entityName} with this {propertyName} not found");
    }

    public static Error AlreadyExists(string memberType, string containerType, string memberName)
    {
        return new($"{containerType}.{memberType}.AlreadyMember",
            ErrorType.Conflict,
            $"{memberName} is already a member of this {containerType}");
    }

    public static Error Archived(string entityName)
    {
        return new($"{entityName}.Archived",
            ErrorType.Forbidden,
            $"{entityName} is archived");
    }

    public static Error Gone(string entityName)
    {
        return new($"{entityName}.Gone",
            ErrorType.Gone,
            $"{entityName} is no longer exists");
    }
}
