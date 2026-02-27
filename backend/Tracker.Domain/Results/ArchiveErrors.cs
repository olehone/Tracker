namespace Tracker.Domain.Results;

public static class ArchiveErrors
{
    public static bool IsArchived(Error error)
    {
        return error.Type == ErrorType.Forbidden
            && error.Code.EndsWith(".Archived", StringComparison.Ordinal);
    }

    public static Error Archived(string entityName)
    {
        return new($"{entityName}.Archived",
            ErrorType.Forbidden,
            $"{entityName} is archived");
    }

    public static Error NotArchivable(string entityName)
    {
        return new($"{entityName}.NotArchivable",
            ErrorType.Conflict,
            $"{entityName} is processed, cannot archive right now");
    }

    public static Error NotUnarchivable(string entityName)
    {
        return new($"{entityName}.NotUnarchivable",
            ErrorType.Forbidden,
            $"{entityName} is processed, cannot unarchive right now");
    }

    public static Error Archiving(string entityName)
    {
        return new($"{entityName}.Archiving",
            ErrorType.Conflict,
            $"{entityName} is processed");
    }
}
