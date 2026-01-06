using Tracker.Domain.Enums;

namespace Tracker.Application.Common.Auth;

public interface IUserContext
{
    Guid GetUserId();
    GlobalRole GetUserRole();
    string GetUserEmail();
    bool IsAuthenticated();
}

