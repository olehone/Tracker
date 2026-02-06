namespace Tracker.Domain.Results;

public enum ErrorType
{
    None,
    Validation,
    NotFound,
    Gone,
    Conflict,
    UniqueViolation,
    ForeignKeyViolation,
    Unauthenticated,
    Forbidden,
    Unknown
}