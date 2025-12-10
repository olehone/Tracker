using Tracker.Application.Results;

namespace Tracker.Persistence;

internal static class PersistenceErrors
{
    internal static readonly Error UniqueViolation = new(
        "Persistence.SaveChangesAsync",
        ErrorType.UniqueViolation);
}
