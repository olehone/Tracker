namespace Tracker.Application.Results;

public enum ErrorType
{
    None,
    Validation,
    NotFound,
    Conflict,
    UniqueViolation,
    ForeignKeyViolation,
    Unknown
}