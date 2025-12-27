using System.Security.Claims;

namespace Tracker.Services.Abstraction.Auth;

public interface IJwtTokenReader
{
    List<Claim> ReadClaims(string token);
    DateTime GetExpirationUtc(string jwt);
}

