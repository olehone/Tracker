using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Tracker.Application.Common.Database;

namespace Tracker.Persistence.Exceptions;

// Is that better to move to Database as in Persistence 
// there no DBMS-related code
// Or write factory / abstract factory for database-specific things
// Or just utility class with all dbms
public class DbExceptionsHandler : IDbExceptionsHandler
{
    public bool IsExceptionDbRelated(Exception ex)
    {
        return ex is SqlException;
    }

    public bool IsExceptionDbUpdate(Exception ex)
    {
        return IsExceptionDbRelated(ex) && ex is DbUpdateException;
    }

    public bool IsExceptionUniqueViolation(Exception ex)
    {
        if (!IsExceptionDbUpdate(ex))
        {
            return false;
        }
        // Can be extended for other DBMS
        return ex switch
        {
            SqlException sqlEx when sqlEx.Number is 2601 or 2627 => true,
            _ => false,
        };
    }
}
