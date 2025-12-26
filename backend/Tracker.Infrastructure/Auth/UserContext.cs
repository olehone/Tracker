using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Tracker.Application.Common.Auth;

namespace Tracker.Infrastructure.Auth;

public class UserContext(IHttpContextAccessor httpContextAccessor)
    : IUserContext
{
    public string GetUserEmail()
    {
        return httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Email    ) ??
            httpContextAccessor.HttpContext?.User.FindFirstValue("email") ??
            throw new InvalidOperationException("User is not authenticated");
    }

    public Guid GetUserId()
    {
        var value =
            httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier) ??
            throw new InvalidOperationException("User is not authenticated");

        return Guid.Parse(value);
    }

    public bool IsAuthenticated()
    {
        return httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
    }
}
