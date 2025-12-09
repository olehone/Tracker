namespace Tracker.Domain.Exceptions;

public class DatabaseException : AppException
{

    public DatabaseException()
    {
    }

    public DatabaseException(string message)
        : base(message)
    {
    }

    public DatabaseException(string message, Exception inner)
        : base(message, inner)
    {
    }

}
