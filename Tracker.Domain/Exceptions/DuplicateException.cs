namespace Tracker.Domain.Exceptions;

public class DuplicateException : DatabaseException
{

    public DuplicateException()
    {
    }

    public DuplicateException(string message)
        : base(message)
    {
    }

    public DuplicateException(string message, Exception inner)
        : base(message, inner)
    {
    }

}
