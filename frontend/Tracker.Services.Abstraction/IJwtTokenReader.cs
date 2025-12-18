namespace Tracker.Services.Abstraction;

public interface IJwtTokenReader
{
    DateTime GetExpirationUtc(string jwt);
}

