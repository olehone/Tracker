using System.Security.Claims;

namespace Tracker.Services.Abstraction;

public interface IJwtTokenReader
{
    List<Claim> ReadClaims(string token);
    DateTime GetExpirationUtc(string jwt);
}

