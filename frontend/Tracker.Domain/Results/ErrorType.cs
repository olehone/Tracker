namespace Tracker.Domain.Results;

public enum ErrorType
{
    None,
    Validation,
    NotFound,
    Gone,
    Conflict,
    Unauthenticated,
    Forbidden,
    Network,
    Server,
    Unknown
}