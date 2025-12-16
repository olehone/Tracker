namespace Tracker.Domain.Results;

public enum ErrorType
{
    None,
    Validation,
    NotFound,
    Conflict,
    UniqueViolation,
    ForeignKeyViolation,
    Unauthorized,
    Unknown
}