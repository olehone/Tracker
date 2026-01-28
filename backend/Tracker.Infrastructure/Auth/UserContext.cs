using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Tracker.Application.Common.Auth;

namespace Tracker.Infrastructure.Auth;

public class UserContext(IHttpContextAccessor httpContextAccessor)
    : IUserContext
{
    private const string anonMessage = "User is not authenticated";

    public Guid GetUserId()
    {
        var idClaim =
            httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier) ??
            throw new InvalidOperationException(anonMessage);

        return Guid.Parse(idClaim);
    }

    public bool IsUnauthenticated()
    {
        return !IsAuthenticated();
    }

    public bool IsAuthenticated()
    {
        return httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
    }
}
