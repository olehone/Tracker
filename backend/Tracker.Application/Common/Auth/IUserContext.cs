using Tracker.Domain.Enums;

namespace Tracker.Application.Common.Auth;

public interface IUserContext
{
    Guid GetUserId();
    bool IsAuthenticated();
    bool IsUnauthenticated();
}

