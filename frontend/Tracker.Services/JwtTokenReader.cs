using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using Tracker.Services.Abstraction;

namespace Tracker.Services;

public sealed class JwtTokenReader : IJwtTokenReader
{
    public DateTime GetExpirationUtc(string jwt)
    {
        var token = new JwtSecurityTokenHandler().ReadJwtToken(jwt);

        var exp = token.Claims
            .First(c => c.Type == JwtRegisteredClaimNames.Exp)
            .Value;

        var seconds = long.Parse(exp, CultureInfo.InvariantCulture);
        return DateTimeOffset.FromUnixTimeSeconds(seconds).UtcDateTime;
    }
}

