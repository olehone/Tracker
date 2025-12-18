namespace Tracker.Application.Common.Auth;

public interface IUserContext
{
    Guid GetUserId();
    string GetUserEmail();
    bool IsAuthenticated();
}

