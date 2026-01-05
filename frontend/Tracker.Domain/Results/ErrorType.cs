namespace Tracker.Domain.Results;

public enum ErrorType
{
    None,
    Validation,
    NotFound,
    Conflict,
    Unauthenticated,
    Forbidden,
    Network,
    Server,
    Unknown
}