namespace Tracker.Domain.Results;

public static class PersistenceErrors
{
    public static readonly Error UniqueViolation = new(
        "Persistence.SaveChangesAsync",
        ErrorType.UniqueViolation);
}
