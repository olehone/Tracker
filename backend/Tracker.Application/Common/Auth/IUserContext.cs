using Tracker.Domain.Enums;
using Tracker.Domain.Results;

namespace Tracker.Application.Common.Auth;

public interface IUserContext
{
    Guid GetUserId();
    Result<GlobalRole> GetUserRole();
    string GetUserEmail();
    bool IsAuthenticated();
}

