namespace Tracker.Domain.Results;

public enum ErrorType
{
    None,
    Validation,
    NotFound,
    Conflict,
    UniqueViolation,
    ForeignKeyViolation,
    Unauthenticated,
    Forbidden,
    Unknown
}