using System.Security.Claims;
using Tracker.Services.Abstraction;
using Tracker.Services.Abstraction.Auth;

namespace Tracker.Services.Auth;

public class CurrentUser(IAuthService authService)
    : ICurrentUser
{
    public bool IsMyId(Guid checkedId)
    {
        return IsAuthenticated && Id == checkedId;
    }

    public bool IsAuthenticated => authService.ClaimsPrincipal.Identity?.IsAuthenticated == true;

    public bool IsUnauthenticated => !IsAuthenticated;

    public Guid Id
    {
        get
        {
            var user = authService.ClaimsPrincipal;
            var claim = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(claim, out var id) 
                ? id
                : throw new InvalidOperationException("Can't get unauthenticated user id ");
        }
    }
}