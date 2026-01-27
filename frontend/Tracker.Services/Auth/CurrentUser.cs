using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Tracker.Services.Abstraction.Auth;

namespace Tracker.Services.Auth;

public class CurrentUser(AuthenticationStateProvider authProvider)
    : ICurrentUser
{
    public bool IsMyId(Guid checkedId)
    {
        return IsAuthenticated && Id == checkedId;
    }

    public bool IsAuthenticated => GetUser()?.Identity?.IsAuthenticated == true;

    public bool IsUnauthenticated => !IsAuthenticated;

    public Guid Id
    {
        get
        {
            var user = GetUser();
            var claim = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(claim, out var id) 
                ? id
                : throw new InvalidOperationException("Can't get unauthenticated user id ");
        }
    }

    private ClaimsPrincipal? GetUser()
    {
        var task = authProvider.GetAuthenticationStateAsync();
        task.Wait();
        return task.Result.User;
    }
}