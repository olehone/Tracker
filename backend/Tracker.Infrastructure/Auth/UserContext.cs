using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Tracker.Application.Common.Auth;
using Tracker.Domain.Enums;

namespace Tracker.Infrastructure.Auth;

public class UserContext(IHttpContextAccessor httpContextAccessor)
    : IUserContext
{
    private const string anonMessage = "User is not authenticated";
    public string GetUserEmail()
    {
        return httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Email) ??
            httpContextAccessor.HttpContext?.User.FindFirstValue("email") ??
            throw new InvalidOperationException(anonMessage);
    }

    public Guid GetUserId()
    {
        var idClaim =
            httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier) ??
            throw new InvalidOperationException(anonMessage);

        return Guid.Parse(idClaim);
    }

    public GlobalRole GetUserRole()
    {
        var roleClaim =
            httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Role) ??
            httpContextAccessor.HttpContext?.User.FindFirstValue("role");

        if (roleClaim is null)
        {
            return GlobalRole.None;
        }

        if (!Enum.TryParse<GlobalRole>(roleClaim, ignoreCase: true, out var globalRole))
        {
            return GlobalRole.None;
        }

        return globalRole;
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
