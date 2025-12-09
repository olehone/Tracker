namespace Tracker.Application.Common.Database;

// To check db related exceptions without writing 
// platform-specific code outside persistence level
public interface IDbExceptionsHandler
{
    bool IsExceptionDbRelated(Exception exception);
    
    // would it be better to make enum in domain
    // with only exceptions i check in code and check this
    // or define general custom exceptions and make transformation
    // handler here 
    bool IsExceptionUniqueViolation(Exception exception);
}
