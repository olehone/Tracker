using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Tracker.Services.Abstraction.Auth;

namespace Tracker.Services.Auth;

public sealed class JwtTokenReader : IJwtTokenReader
{
    private static readonly JwtSecurityTokenHandler Handler = new();

    public DateTime GetExpirationUtc(string jwt)
    {
        var token = Handler.ReadJwtToken(jwt);
        return token.ValidTo;
    }

    public List<Claim> ReadClaims(string token)
    {
        var jwt = Handler.ReadJwtToken(token);
        return jwt.Claims.ToList();
    }
}